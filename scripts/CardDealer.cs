using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardDealer : Node2D {
	public DrawPile DrawPile { get; set; }
	public Marker2D DiscardPile { get; set; }
	public Marker2D Hand { get; set; }
	public Marker2D OpponentHand { get; set; }
	private Node _screensContainer;

	public override void _Ready() {
		DrawPile = (DrawPile)GetNode("%DrawPile");
		DiscardPile = (Marker2D)GetNode("%DiscardPile");
		Hand = (Marker2D)GetNode("%Hand");
		OpponentHand = (Marker2D)GetNode("%OpponentHand");
		_screensContainer = GetNode<Node>("%ScreensContainer");
	}

	private void ManageCards(Marker2D owner, string[] cards = null, bool createNew = false) {
		int cardCount = createNew ? cards.Length : owner.GetChildCount();
		float cardOffsetX = 16f;
		float rotMax = Mathf.Pi / 10;
		
		float step = 20f / (cardCount + 1);
		if (cardCount == 1) step = 0;

		float pos = -10f + step;
		float k = 2f;
		float offset = k * Mathf.Sqrt(100 - (step - 10) * (step - 10));

		for (int i = 0; i < cardCount; i++, pos += step) {
			Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
			Card card;

			if (createNew) {
				card = (Card)Scenes.Card.Instantiate();
				card.CreateCard(cards[i]);
				
				card.Connect(Card.SignalName.ColorCardClicked, new Callable(this, MethodName.PlayCard));
				card.Connect(Card.SignalName.WildCardClicked, new Callable(this, MethodName.HandleWildCard));

				owner.AddChild(card);
				card.GlobalPosition = DrawPile.GlobalPosition;
			} else {
				card = (Card)owner.GetChild(i);
			}

			Vector2 finalPos = -(card.Size / 2) - new Vector2((float)(cardOffsetX * (cardCount - 1 - i)), k * Mathf.Sqrt(100 - pos * pos) - offset);
			finalPos.X += cardOffsetX * (cardCount - 1) / 2;

			var rotRadians = cardCount > 1 ? Mathf.LerpAngle(-rotMax, rotMax, (double)i / (cardCount - 1)) : 0;

			tween.Parallel().TweenProperty(card, "position", finalPos, 0.3 + (i * 0.075));
			tween.Parallel().TweenProperty(card, "rotation", rotRadians, 0.3 + (i * 0.075));
		}
	}

	public void DrawCards(Marker2D owner, string[] cards) {
		ManageCards(owner, cards, true);
		DrawPile.UpdateCardsNumber();
	}

	public void ArrangeCards(Marker2D owner) {
		ManageCards(owner);
	}

	public void CheckCardsAvailability() {
		foreach (var card in Hand.GetChildren().Cast<Card>()) {
			if (card.CanBePlayed()) return;
		}

		if (GameInfo.DeckCardsNumber != 0) DrawPile.UnlockDrawFeature();		
	}

	public void PlayCard(Marker2D from, Card card) {
		Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);

		if (card.GetParent() is null) DiscardPile.AddChild(card);
		else card.Reparent(DiscardPile);
		card.ObliterateFromTheFaceOfTheEarth();
		GameInfo.PlayedCard = card;

		card.GlobalPosition = from.GlobalPosition - (card.Size / 2);
		var finalPos = DiscardPile.GlobalPosition - (card.Size / 2);

		tween.TweenProperty(card, "global_position", finalPos, 0.3);
		ArrangeCards(Hand);
	}

	public void HandleWildCard(Card card) {
		if (!PlayerInfo.IsYourTurn) return;

		var wildMenu = (WildMenu)Scenes.WildMenu.Instantiate();
		_screensContainer.AddChild(wildMenu);

		wildMenu.Connect(WildMenu.SignalName.ColorChoosen, new Callable(card, Card.MethodName.HandleWildCard));
	}
}
