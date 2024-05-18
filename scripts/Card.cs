using Godot;
using Godot.Collections;
using Newtonsoft.Json.Linq;
using System;

public partial class Card : Button {
	[Signal] public delegate void WildCardClickedEventHandler(Marker2D from, Card card);
	[Signal] public delegate void ColorCardClickedEventHandler(Marker2D from, Card card);
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
	private TextureRect _marker;
	private AnimationPlayer _animPlayer;

	public override void _Ready() {
		_marker = GetNode<TextureRect>("%Marker");
		_animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		Connect(SignalName.Pressed, Callable.From(OnCardClicked));
		Connect(SignalName.MouseEntered, Callable.From(OnMouseEntered));
		Connect(SignalName.MouseExited, Callable.From(OnMouseExited));

		_animPlayer.Play("bounce");
	}

    public override void _Process(double delta) {
		Disabled = !CanBePlayed();
		_marker.Visible = CanBePlayed();
    }

    public void OnCardClicked() {
		if (Color == CardColor.Wild) {
			EmitSignal(SignalName.WildCardClicked, this);
		} else {
			EmitSignal(SignalName.ColorCardClicked, GetParent(), this);
			
			MyDTO playedCard = new() {
				RequestType = "Play Card",
				Data = new {
					Name = Name,
					Color = GetColorString(),
					Value = Value
				}
			};

			GameInfo.Queue.Enqueue(playedCard);
		}
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

		textureNode.Texture = (AtlasTexture)GD.Load("res://resources/cards/" + Value + ".tres");
		backgroundNode.Color = CardColors[Color];

		Position = -(Size / 2);
	}

	public void CreateCard(CardColor color, string value) {
		Color = color;
		Value = value;
		Name = GetColorString() + '_' + value;

		var textureNode = (TextureRect)GetNode("Texture");
		var backgroundNode = (ColorRect)GetNode("Background");

		textureNode.Texture = (AtlasTexture)GD.Load("res://resources/cards/" + Value + ".tres");
		backgroundNode.Color = CardColors[Color];

		Position = -(Size / 2);
	}

	public bool CanBePlayed() => 
		GameInfo.PlayedCard is not null &&
		Name != "Wild_Back" &&
		PlayerInfo.IsYourTurn && 
		(GameInfo.PlayedCard.Color == CardColor.Wild ||
		Color == CardColor.Wild ||
		Color == GameInfo.PlayedCard.Color ||
		Value == GameInfo.PlayedCard.Value);

	public void HandleWildCard(CardColor color) {
		Color = color;
		if (Value.StartsWith("Wild")) Value = Value[4..];
		CreateCard(Color, Value);
		
		OnCardClicked();
	}

	public string GetColorString() => Enum.GetName(typeof(CardColor), Color);

	public void ObliterateFromTheFaceOfTheEarth() {
		Disconnect(SignalName.MouseEntered, Callable.From(OnMouseEntered));
		Disconnect(SignalName.MouseExited, Callable.From(OnMouseExited));
		SetProcess(false);
		Disabled = true;
		_animPlayer.Stop();
		_marker.Visible = false;
	}

	public void OnMouseEntered() {
		Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
		tween.Parallel().TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.1);

		ZIndex = 1;
	}

	public void OnMouseExited() {
		Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
		tween.Parallel().TweenProperty(this, "scale", new Vector2(1f, 1f), 0.3);

		ZIndex = 0;
	}

}

public enum CardColor {
	Red,
	Yellow,
	Green,
	Blue,
	Wild
}
