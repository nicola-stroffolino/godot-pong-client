using Godot;
using System;

public partial class WildMenuButton : TextureButton {
	[Signal] public delegate void ColorButtonPressedEventHandler(CardColor value);
	[Export] public CardColor Value { get; set; }

	public override void _Ready() {
		Connect(SignalName.MouseEntered, Callable.From(OnMouseEntered));
		Connect(SignalName.MouseExited, Callable.From(OnMouseExited));
		Connect(SignalName.Pressed, Callable.From(OnButtonPressed));
	}

	public void OnMouseEntered() {
		Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
		tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.1);
	}

	public void OnMouseExited() {
		Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
		tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.3);
	}

	public void OnButtonPressed() {
		EmitSignal(SignalName.ColorButtonPressed, (int)Value);
	}
}
