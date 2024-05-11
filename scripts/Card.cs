using Godot;
using System;

public partial class Card : Button {
	public CardData Data { get; set; }
	private bool _holding = false;

	public override void _Ready() {
		(GetNode("Texture") as TextureRect).Texture = Data.Texture;
		Connect(SignalName.ButtonDown, new Callable(this, MethodName.OnButtonDown));
		Connect(SignalName.ButtonUp, new Callable(this, MethodName.OnButtonUp));
	}

	public override void _Process(double delta) {
		if (_holding) Position = GetViewport().GetMousePosition();
	}

	public void OnButtonDown() => _holding = true;

	public void OnButtonUp() => _holding = false;
}
