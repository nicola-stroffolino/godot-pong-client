using Newtonsoft.Json;
using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public partial class Game : Node2D {
	[Signal] public delegate void GameStartedEventHandler();
	[Signal] public delegate void CardPlacedEventHandler();
	[Signal] public delegate void OppCardsChangedEventHandler();
	[Signal] public delegate void TurnStartedEventHandler();
	[Signal] public delegate void CardDrawnEventHandler();
	private Label _cardColorLbl;
	private Label _cardValueLbl;
	private Label _turnLbl;
	private CardDealer _cardDealer;

	public override void _Ready(){
		GetNode<Label>("%Room").Text = PlayerInfo.ConnectedRoomName;
		GetNode<Label>("%Opponent").Text = PlayerInfo.Opponent;
		GetNode<Label>("%Nickname").Text = PlayerInfo.Nickname;
		_cardColorLbl = GetNode<Label>("%CardColor");
		_cardValueLbl = GetNode<Label>("%CardValue");
		_turnLbl = GetNode<Label>("%Turn");
		_cardDealer = GetNode<CardDealer>("%CardDealer");

		// Connect(SignalName.GameStarted, new Callable(_cardDealer, CardSpawner.MethodName.StartGame));
		// Connect(SignalName.CardPlaced, new Callable(_cardDealer, CardSpawner.MethodName.PlacePlayedCard));
		// Connect(SignalName.OppCardsChanged, new Callable(_cardDealer, CardSpawner.MethodName.ChangeOppCardsNumber));
		// Connect(SignalName.TurnStarted, new Callable(_cardDealer, CardSpawner.MethodName.CheckCardsAvailability));
		// Connect(SignalName.CardDrawn, new Callable(_cardDealer, CardSpawner.MethodName.CheckCardsAvailability));
	}

	public override void _Process(double delta) {
		GameInfo.Ws.Poll();
		if (GameInfo.Ws.GetReadyState() != WebSocketPeer.State.Open) return;


		if (GameInfo.PlayedCard is not null) {
			_cardColorLbl.Text = Enum.GetName(typeof(CardColor), GameInfo.PlayedCard.Color);
			_cardValueLbl.Text = GameInfo.PlayedCard.Value;
		}

		if (PlayerInfo.IsYourTurn) _turnLbl.Text = "Its your turn!";
		else _turnLbl.Text = "Wait for the other player's turn...";
			
		/* --- Send Data --- */
		if(GameInfo.Queue.Count != 0) {
			var el = GameInfo.Queue.Dequeue();
			var json = JsonConvert.SerializeObject(el);
			GameInfo.Ws.PutPacket(json.ToUtf8Buffer());
		}

		/* --- Receive Data --- */
		var message = System.Text.Encoding.Default.GetString(GameInfo.Ws.GetPacket());
		var payload = JsonConvert.DeserializeObject<JObject>(message);

		if (payload is null) return;

		GD.Print(payload);

		switch ((string)payload["responseType"]) {
			case "Game Started": 
			{	
				GetNode<Label>("%Opponent").Text = PlayerInfo.Opponent = (string)payload["opponentNickname"];

				GameInfo.PlayedCard = (Card)Scenes.Card.Instantiate();
				GameInfo.PlayedCard.CreateCard((string)payload["discardPile"]);
				GameInfo.PlayedCard.Disabled = true;
				_cardDealer.DiscardPile.AddChild(GameInfo.PlayedCard);

				var drawPileCard = (Card)Scenes.Card.Instantiate();
				drawPileCard.CreateCard("Wild_Back");
				_cardDealer.DrawPile.AddChild(drawPileCard);

				var cards = payload["cardsDrawn"].ToObject<string[]>();
				_cardDealer.DrawCards(_cardDealer.Hand, cards.Length, cards);
				_cardDealer.DrawCards(_cardDealer.OpponentHand, (int)payload["opponentCardsDrawnNumber"]);

				break;
			}
			case "Card Played": {
				if (!PlayerInfo.IsYourTurn) {
					var oppPlayedCard = (Card)_cardDealer.OpponentHand.GetChild(0);
					oppPlayedCard.CreateCard((string)payload["discardPile"]);
					_cardDealer.PlayCard(_cardDealer.OpponentHand, oppPlayedCard);
				}

				PlayerInfo.IsYourTurn = (bool)payload["yourTurn"];
				
				break;
			}
			case "Change Turn": {
				break;
			}
			default: break;
		}		

		// else if (payload["draw"] is not null && payload["oppDrawNumber"] is not null && payload["playedCard"] is not null) {
		// 	EmitSignal(
		// 		SignalName.GameStarted, 
		// 		payload["draw"].ToObject<string[]>(), 
		// 		(int)payload["oppDrawNumber"], 
		// 		(string)payload["playedCard"]
		// 	);
		// } else if (payload["newColor"] is not null && payload["newValue"] is not null) {
		// 	EmitSignal(
		// 		SignalName.CardPlaced,
		// 		(string)payload["newColor"],
		// 		(string)payload["newValue"]
		// 	);
		// } else if (payload["yourTurn"] is not null) {
		// 	PlayerInfo.IsYourTurn = (bool)payload["yourTurn"];
		// 	EmitSignal(SignalName.TurnStarted);
		// } else if (payload["oppCardsNumber"] is not null) {
		// 	EmitSignal(SignalName.OppCardsChanged, (int)payload["oppCardsNumber"]);
		// } else if (payload["draw"] is not null) {
		// 	PackedScene cardScn = GD.Load("res://scenes/card.tscn") as PackedScene;
		// 	GD.Print(payload["draw"]);
		// 	foreach (var drawnCard in payload["draw"].ToObject<string[]>()) {
		// 		var newCard = (Card)cardScn.Instantiate();
		// 		newCard.CreateCard(drawnCard);
		// 		_cardDealer.AddCard(newCard);
		// 	}
			
		// 	EmitSignal(SignalName.CardDrawn);
		// }
		
	}

	public override void _ExitTree() {
		GameInfo.Ws.Close();
	}
}


