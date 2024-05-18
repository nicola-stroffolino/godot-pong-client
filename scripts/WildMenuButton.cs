using Godot;
using System;

public partial class WildMenuButton : TextureButton {
	[Signal] public delegate void ColorButtonPressedEventHandler(CardColor value);
	[Export] public CardColor Value { get; set; }

	public override void _Ready() {
		Connect(SignalName.Pressed, Callable.From(OnButtonPressed));
	}
	
	public void OnButtonPressed() {
		EmitSignal(SignalName.ColorButtonPressed, (int)Value);
	}
}
