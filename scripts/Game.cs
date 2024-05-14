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
	private Label _cardColorLbl;
	private Label _cardValueLbl;
	private Label _turnLbl;
	private CardSpawner _cardSpawner;

	public override void _Ready(){
		GetNode<Label>("%Room").Text = PlayerInfo.ConnectedRoomName;
		GetNode<Label>("%Opponent").Text = PlayerInfo.Opponent;
		GetNode<Label>("%Nickname").Text = PlayerInfo.Nickname;
		_cardColorLbl = GetNode<Label>("%CardColor");
		_cardValueLbl = GetNode<Label>("%CardValue");
		_turnLbl = GetNode<Label>("%Turn");
		_cardSpawner = GetNode<CardSpawner>("%CardSpawner");

		Connect(SignalName.GameStarted, new Callable(_cardSpawner, CardSpawner.MethodName.StartGame));
		Connect(SignalName.CardPlaced, new Callable(_cardSpawner, CardSpawner.MethodName.PlacePlayedCard));
		Connect(SignalName.OppCardsChanged, new Callable(_cardSpawner, CardSpawner.MethodName.ChangeOppCardsNumber));
		Connect(SignalName.TurnStarted, new Callable(_cardSpawner, CardSpawner.MethodName.ChangeOppCardsNumber));
	}

	public override void _Process(double delta) {
		GameInfo.Ws.Poll();
		
		if (GameInfo.PlayedCard is not null) {
			_cardColorLbl.Text = Enum.GetName(typeof(CardColor), GameInfo.PlayedCard.Color);
			_cardValueLbl.Text = GameInfo.PlayedCard.Value;
		}

		if (PlayerInfo.IsYourTurn) _turnLbl.Text = "Its your turn!";
		else _turnLbl.Text = "Wait for the other player's turn...";
				
		if(GameInfo.Ws.GetReadyState() == WebSocketPeer.State.Open) {
			
			/* --- Send Data --- */
			if(GameInfo.Queue.Count != 0) {
				var el = GameInfo.Queue.Dequeue();
				var json = JsonConvert.SerializeObject(el);
				GameInfo.Ws.PutPacket(json.ToUtf8Buffer());
			}	

			/* --- Receive Data --- */
			var message = System.Text.Encoding.Default.GetString(GameInfo.Ws.GetPacket());
			var payload = JsonConvert.DeserializeObject<JObject>(message);

			if (payload is not null) {
				if (payload["oppNickname"] is not null) {
					GetNode<Label>("%Opponent").Text = PlayerInfo.Opponent = (string)payload["oppNickname"];
				} else if (payload["draw"] is not null && payload["oppDrawNumber"] is not null && payload["playedCard"] is not null) {
					EmitSignal(
						SignalName.GameStarted, 
						payload["draw"].ToObject<string[]>(), 
						(int)payload["oppDrawNumber"], 
						(string)payload["playedCard"]
					);
				} else if (payload["newColor"] is not null && payload["newValue"] is not null) {
					EmitSignal(
						SignalName.CardPlaced,
						(string)payload["newColor"],
						(string)payload["newValue"]
					);
				} else if (payload["yourTurn"] is not null) {
					PlayerInfo.IsYourTurn = (bool)payload["yourTurn"];
					EmitSignal(SignalName.TurnStarted);
				} else if (payload["oppCardsNumber"] is not null) {
					EmitSignal(SignalName.OppCardsChanged, (int)payload["oppCardsNumber"]);
				}
			}
		}
	}

	public override void _ExitTree() {
		GameInfo.Ws.Close();
	}
}


