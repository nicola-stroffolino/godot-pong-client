using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardDealer : Node2D {
	public Marker2D DrawPile { get; set; }
	public Marker2D DiscardPile { get; set; }
	public Marker2D Hand { get; set; }
	public Marker2D OpponentHand { get; set; }

	public override void _Ready() {
		DrawPile = (Marker2D)GetNode("%DrawPile");
		DiscardPile = (Marker2D)GetNode("%DiscardPile");
		Hand = (Marker2D)GetNode("%Hand");
		OpponentHand = (Marker2D)GetNode("%OpponentHand");
	}

	public override void _Process(double delta) {

	}

	public void DespawnCard(Card card, Marker2D parent) {
		card.QueueFree();
	}

	public void DrawCards(Marker2D owner, int number, string[] cards = null) {
		float cardOffsetX = 16f;
		float rotMax = Mathf.Pi / 10;
		Tween _tween = null;
		
		if (_tween is not null && _tween.IsRunning()) _tween.Kill();
		_tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);

		float step = 20f / (number + 1);
		if (number == 1) step = 0;
		float pos = -10f + step;
		float k = 2f;
		float offset = k * Mathf.Sqrt(100 - (step - 10) * (step - 10));
		for (int i = 0; i < number; i++, pos += step) {
			var newCard = (Card)Scenes.Card.Instantiate();
			if (cards is null) newCard.CreateCard("Wild_Back");
			else newCard.CreateCard(cards[i]);

			newCard.Connect(Card.SignalName.CardClicked, new Callable(this, MethodName.PlayCard));	
			owner.AddChild(newCard);
			newCard.GlobalPosition = DrawPile.GlobalPosition;

			Vector2 finalPos = -(newCard.Size / 2) - new Vector2((float)(cardOffsetX * (number - 1 - i)), k * Mathf.Sqrt(100 - pos * pos) - offset );
			finalPos.X += cardOffsetX * (number - 1) / 2;

			var rotRadians = Mathf.LerpAngle(-rotMax, rotMax, (double)i / (number - 1));

			_tween.Parallel().TweenProperty(newCard, "position", finalPos, 0.3 + (i * 0.075));
			_tween.Parallel().TweenProperty(newCard, "rotation", rotRadians, 0.3 + (i * 0.075));
		}
	}

	[Obsolete("Holding this method for reference, not working..")]
	public void StartGame(string[] drawnCards, int opponentDrawNumber, string playedCardString) {
		PackedScene cardScn = GD.Load("res://scenes/card.tscn") as PackedScene;
					
		foreach (var cardName in drawnCards) {
			var newCard = (Card)cardScn.Instantiate();
			newCard.CreateCard(cardName);
			newCard.Connect(Card.SignalName.CardClicked, new Callable(this, MethodName.DespawnCard));
			
			Hand.AddChild(newCard);
		}
		
		for (int i = 0; i < opponentDrawNumber; i++) {
			var newCard = (Card)cardScn.Instantiate();
			newCard.CreateCard("Wild_Back");
			
			OpponentHand.AddChild(newCard);
		}

		var playedCard = (Card)cardScn.Instantiate();
		playedCard.CreateCard(playedCardString);
		GameInfo.PlayedCard = playedCard;
		DiscardPile.AddChild(playedCard);
	}

	public void PlacePlayedCard(string color, string value) {
		PackedScene cardScn = GD.Load("res://scenes/card.tscn") as PackedScene;

		GameInfo.PlayedCard = (Card)cardScn.Instantiate();
		GameInfo.PlayedCard.CreateCard($"{color}_{value}");
		
		DiscardPile.AddChild(GameInfo.PlayedCard);
	}

	public void ChangeOpponentCardsNumber(int cardsNumber) {
		foreach (var card in OpponentHand.GetChildren()) {
			OpponentHand.RemoveChild(card);
			card.QueueFree();
		}
		
		PackedScene cardScn = GD.Load("res://scenes/card.tscn") as PackedScene;

		for (int i = 0; i < cardsNumber; i++) {
			var newCard = (Card)cardScn.Instantiate();
			newCard.CreateCard("Wild_Back");
			
			OpponentHand.AddChild(newCard);
		}

	}

	public void CheckCardsAvailability() {
		bool atLeastOneCardPlayable = false;
		foreach (var card in Hand.GetChildren().Cast<Card>()) {
			if (card.CanBePlayed()) atLeastOneCardPlayable = true;
		}

		if (!atLeastOneCardPlayable) {
			MyDTO drawCard = new() {
				RequestType = "Draw Card",
				Data = new {
					Quantity = 1
				}
			};

			GameInfo.Queue.Enqueue(drawCard);
		}
	}

	public void AddCard(Card card) {
		Hand.AddChild(card);
	}

	public void PlayCard(Marker2D from, Card card) {
		Tween _tween = null;
		
		if (_tween is not null && _tween.IsRunning()) _tween.Kill();
		_tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);

		card.Disabled = true;
		card.Reparent(DiscardPile);
		card.GlobalPosition = from.GlobalPosition - (card.Size / 2);

		var finalPos = DiscardPile.GlobalPosition - (card.Size / 2);

		_tween.Parallel().TweenProperty(card, "global_position", finalPos, 0.3);
	}

	// public void PlayCard(Marker2D from, string card) {
	// 	Tween _tween = null;
		
	// 	if (_tween is not null && _tween.IsRunning()) _tween.Kill();
	// 	_tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);

	// 	var newCard = (Card)Scenes.Card.Instantiate();
	// 	newCard.CreateCard(card);
	// 	DiscardPile.AddChild(newCard);
	// 	newCard.GlobalPosition = from.GlobalPosition - (newCard.Size / 2);

	// 	var finalPos = DiscardPile.GlobalPosition - (newCard.Size / 2);

	// 	_tween.Parallel().TweenProperty(newCard, "global_position", finalPos, 0.3);
	// }
}
