using Godot;
using System;

[GlobalClass]
public partial class CardData : Resource {
	[Export] public int Value { get; set; }
	[Export] public AtlasTexture Texture { get; set; }
}

