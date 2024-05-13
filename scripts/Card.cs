using Godot;
using Godot.Collections;
using Newtonsoft.Json.Linq;
using System;

public partial class Card : Button {
	[Signal] public delegate void CardClickedEventHandler(Card card, Marker2D parent);
	public Dictionary<CardColor, Color> CardColors { get; set; } = new() {
		{ CardColor.Red, 	new Color(0xD80000FF) },
		{ CardColor.Yellow, new Color(0xF8A000FF) },
		{ CardColor.Green, 	new Color(0x00C000FF) },
		{ CardColor.Blue, 	new Color(0x0000C0FF) },
		{ CardColor.Wild, 	new Color(0xFFFFFFFF) }
	};
	public new string Name { get; set; }
	public string Value { get; set; }
	public CardColor Color { get; set; }

	public override void _Ready() {
		Connect(SignalName.Pressed, new Callable(this, MethodName.OnCardClicked));
	}

	public override void _Process(double delta) {
		Disabled = !CanBePlayed();
	}

	public void OnCardClicked() {
		EmitSignal(SignalName.CardClicked, this, GetParent());

		MyDTO playedCard = new() {
			RequestType = "Play Card",
			Data = new {
				Name = Name,
				Color = Color,
				Value = Color
			}
		};

		GD.Print(Name, Color, Color);

		GameInfo.Queue.Enqueue(playedCard);
	}

	public void CreateCard(string uniqueName) {
		var cardInfo = uniqueName.Split('_');
		var color = cardInfo[0];
		var value = cardInfo[1];

		Name = uniqueName;
		Color = (CardColor)Enum.Parse(typeof(CardColor), color);
		Value = value;

		var textureNode = GetNode("Texture") as TextureRect;
		var backgroundNode = GetNode("Background") as ColorRect;

		textureNode.Texture = (AtlasTexture)GD.Load("res://resources/textures/" + Value + ".tres");
		backgroundNode.Color = CardColors[Color];
	}

	public bool CanBePlayed() => 
		PlayerInfo.IsYourTurn &&
		(GameInfo.PlayedCard == null ||
		Color == GameInfo.PlayedCard.Color ||
		Color == CardColor.Wild ||
		Value == GameInfo.PlayedCard.Value);
}

public enum CardColor {
	Red,
	Yellow,
	Green,
	Blue,
	Wild
}
