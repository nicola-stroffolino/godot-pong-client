using Godot;

[GlobalClass]
public partial class ActionCardData : CardData {
	[Export] public CardAction Action { get; set; }
}