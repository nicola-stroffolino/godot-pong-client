using Newtonsoft.Json;
using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public partial class Game : Node2D {
	private Label _cardColorLbl;
	private Label _cardValueLbl;
	public override void _Ready(){
		GetNode<Label>("%Room").Text = PlayerInfo.ConnectedRoomName;
		GetNode<Label>("%Opponent").Text = PlayerInfo.Opponent;
		GetNode<Label>("%Nickname").Text = PlayerInfo.Nickname;
		_cardColorLbl = GetNode<Label>("%CardColor");
		_cardValueLbl = GetNode<Label>("%CardValue");
	}

	public override void _Process(double delta) {
		GameInfo.Ws.Poll();
		if (GameInfo.PlayedCard is not null) {
			_cardColorLbl.Text = Enum.GetName(typeof(CardColor), GameInfo.PlayedCard.Color);
			_cardValueLbl.Text = GameInfo.PlayedCard.Value;
		}
				
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
				}
			}
		}
	}

	public override void _ExitTree() {
		GameInfo.Ws.Close();
	}
}


