using Godot;
using System;

[GlobalClass]
public partial class CardData : Resource {
	public string Value { get; set; }
	public CardColor Color { get; set; }	
}

public enum CardColor {
	Red,
	Yellow,
	Green,
	Blue,
	Wild
}
