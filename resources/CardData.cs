using Godot;
using System;

[GlobalClass]
public partial class CardData : Resource {
	[Export] public CardColor Color { get; set; }
	[Export] public AtlasTexture Texture { get; set; } = (AtlasTexture)GD.Load("res://resources/textures/cards_atlas_texture.tres");
	
}

public enum CardAction {
	Skip,
	Reverse,
	Draw_2,
	Draw_4	
}

public enum CardColor {
	Wild,
	Red,
	Yellow,
	Green,
	Blue
}
