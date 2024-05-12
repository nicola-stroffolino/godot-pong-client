using Godot;
using Godot.Collections;
using System;

public partial class Card : Button {
	public Dictionary<CardColor, Color> CardColors { get; set; } = new() {
		{ CardColor.Red, 	new Color(0xD80000FF) },
		{ CardColor.Yellow, new Color(0xF8A000FF) },
		{ CardColor.Green, 	new Color(0x00C000FF) },
		{ CardColor.Blue, 	new Color(0x0000C0FF) },
		{ CardColor.Wild, 	new Color(0xFFFFFFFF) }
	};
	public CardData Data { get; set; }

	public override void _Ready() {
		// Connect(SignalName.ButtonDown, new Callable(this, MethodName.OnButtonDown));
		// Connect(SignalName.ButtonUp, new Callable(this, MethodName.OnButtonUp));
	}

	public override void _Process(double delta) {
	}

	// public void OnButtonDown();

	// public void OnButtonUp();

	public Card CreateCard(CardData data) {
		Data = data;
		var textureNode = GetNode("Texture") as TextureRect;
		var backgroundNode = GetNode("Background") as ColorRect;

		textureNode.Texture = (AtlasTexture)GD.Load("res://resources/textures/" + data.Value + ".tres");
		backgroundNode.Color = CardColors[data.Color];

		return (Card)Duplicate();
	}
}
