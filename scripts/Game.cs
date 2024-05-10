using Newtonsoft.Json;
using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public partial class Game : Node2D {
	// private string _socketUrl = "ws://localhost:5000";
	// private readonly WebSocketPeer _ws = new();

	// private readonly RandomNumberGenerator _rng = new();
	// private readonly Queue<MyDTO> _queue = new();

	// private readonly Room _connectedRoom;
	private PackedScene _player = (PackedScene)GD.Load("res://scenes/player.tscn");
	private PackedScene _enemy = (PackedScene)GD.Load("res://scenes/enemy.tscn");

	private Player _plr;
	private Enemy _enm;
	// private Array<CharacterBody2D> _enemies = new();

	// [ExportGroup("Room Inputs")]
	// [Export]
	// public LineEdit RoomName { get; set; }
	// [Export]
	// public LineEdit RoomPassword { get; set; } 

	public override void _Ready(){
		GetNode<Label>("%RoomId").Text = PlayerInfo.ConnectedRoomName;
		GetNode<Label>("%PlayerId").Text = PlayerInfo.Id.ToString();
		GetNode<Label>("%Nickname").Text = PlayerInfo.Nickname;

		_plr = _player.Instantiate() as Player;
		AddChild(_plr);
	}

	public override void _Process(double delta) {
		GameInfo.Ws.Poll();
		
		// switch (GameInfo.Ws.GetReadyState()) {
		// 	case WebSocketPeer.State.Connecting:
		// 		GD.Print("Connecting to server...");
		// 		break;
		// 	case WebSocketPeer.State.Open:
		// 		// GD.Print("Connected to server.");
				
		// 		if(GameInfo.Queue.Count != 0) {
		// 			var el = GameInfo.Queue.Dequeue();
		// 			var json = JsonConvert.SerializeObject(el);
		// 			GameInfo.Ws.PutPacket(json.ToUtf8Buffer());
		// 		}
		
		if(GameInfo.Ws.GetReadyState() == WebSocketPeer.State.Open) {
			
			/* --- Send Data --- */
			MyDTO _playerData = new() {
				RequestType = "Player Move",
				Data = new {
					// roomName = PlayerInfo.ConnectedRoomName,
					id = PlayerInfo.Id,
					x = _plr.Position.X,
					y = _plr.Position.Y
				}
			};

			var json = JsonConvert.SerializeObject(_playerData);
			GameInfo.Ws.PutPacket(json.ToUtf8Buffer());

			/* --- Receive Data --- */
			var message = System.Text.Encoding.Default.GetString(GameInfo.Ws.GetPacket());
			var payload = JsonConvert.DeserializeObject<JObject>(message);

			if (payload is not null) {
				if (_enm is null) {
					_enm = _enemy.Instantiate() as Enemy;
					AddChild(_enm);
				}
				_enm.Position = new Vector2((float)payload["x"], (float)payload["y"]);
			}

		}


				
		// 		break;
		// 	case WebSocketPeer.State.Closing:
		// 		GD.Print("Disconnecting from server...");
		// 		break;
		// 	case WebSocketPeer.State.Closed:
		// 		GD.Print("Not connected to server.");
		// 		break;
		// }
	}

	public override void _ExitTree() {
		GameInfo.Ws.Close();
	}

	class PlayerData {
		public float x { get; set; } = 0;
		public float y { get; set; } = 0;
		public int id { get; set; } = 0;
	}

	class Room {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public int MaxPlayers { get; set; } = 2;
	}

	class MyDTO {
		public string RequestType { get; set; }
		public object Data { get; set; } // JSON
	}
}


