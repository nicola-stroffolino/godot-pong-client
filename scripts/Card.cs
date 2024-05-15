using Godot;
using Godot.Collections;
using Newtonsoft.Json.Linq;
using System;

public partial class Card : Button {
	[Signal] public delegate void CardClickedEventHandler(Marker2D from, Card card);
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

	private Tween _tween;

	public override void _Ready() {
		Connect(SignalName.Pressed, new Callable(this, MethodName.OnCardClicked));
	}

	public override void _Process(double delta) {
		Disabled = !CanBePlayed();
	}

	public void OnCardClicked() {
		EmitSignal(SignalName.CardClicked, GetParent(), this);

		MyDTO playedCard = new() {
			RequestType = "Play Card",
			Data = new {
				Name = Name,
				Color = GetColorString(),
				Value = Value
			}
		};

		GameInfo.Queue.Enqueue(playedCard);
		// PlayerInfo.IsYourTurn = false;
	}

	public void CreateCard(string uniqueName) {
		var cardInfo = uniqueName.Split('_');
		var color = cardInfo[0];
		var value = cardInfo[1];

		Name = uniqueName;
		Color = (CardColor)Enum.Parse(typeof(CardColor), color);
		Value = value;

		var textureNode = (TextureRect)GetNode("Texture");
		var backgroundNode = (ColorRect)GetNode("Background");

		textureNode.Texture = (AtlasTexture)GD.Load("res://resources/textures/" + Value + ".tres");
		backgroundNode.Color = CardColors[Color];

		Position = -(Size / 2);
	}

	public bool CanBePlayed() => 
		PlayerInfo.IsYourTurn && GameInfo.PlayedCard is not null &&
		(GameInfo.PlayedCard.Color == CardColor.Wild ||
		Color == CardColor.Wild ||
		Color == GameInfo.PlayedCard.Color ||
		Value == GameInfo.PlayedCard.Value);

	public string GetColorString() => Enum.GetName(typeof(CardColor), Color);
}

public enum CardColor {
	Red,
	Yellow,
	Green,
	Blue,
	Wild
}
