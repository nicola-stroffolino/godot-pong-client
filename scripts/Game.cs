using Newtonsoft.Json;
using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.Collections.Generic;

public partial class Game : Node2D {
	// private string _socketUrl = "ws://localhost:5000";
	// private readonly WebSocketPeer _ws = new();

	// private readonly PlayerData _playerData = new();
	// private readonly RandomNumberGenerator _rng = new();
	// private readonly Queue<MyDTO> _queue = new();

	// private readonly Room _connectedRoom;
	private PackedScene _enemy = (PackedScene)GD.Load("res://scenes/Enemy.tscn");
	private Array<CharacterBody2D> _enemies = new();

	// [ExportGroup("Room Inputs")]
	// [Export]
	// public LineEdit RoomName { get; set; }
	// [Export]
	// public LineEdit RoomPassword { get; set; } 

	public override void _Ready(){

	}

	// public void OnJoinButtonPressed() {
	// 	var err = GameInfo.Ws.ConnectToUrl(GameInfo.SocketUrl);
	// 	if (err != Error.Ok) {
	// 		GD.Print("Connection Refused");
	// 		return;
	// 	}

	// 	_playerData.id = _rng.RandiRange(1, 100);
	// 	_rng.Randomize();

	// 	MyDTO joinRoom = new() {
	// 		RequestType = "Join Room",
	// 		Data = new {
	// 			Name = RoomName.Text,
	// 			Password = RoomPassword.Text
	// 		}
	// 	};

	// 	_queue.Enqueue(joinRoom);
	// }


	public override void _Process(double delta) {
		GameInfo.Ws.Poll();
		
		switch (GameInfo.Ws.GetReadyState()) {
			case WebSocketPeer.State.Connecting:
				GD.Print("Connecting to server...");
				break;
			case WebSocketPeer.State.Open:
				// GD.Print("Connected to server.");
				
				if(GameInfo.Queue.Count != 0) {
					var el = GameInfo.Queue.Dequeue();
					var json = JsonConvert.SerializeObject(el);
					GameInfo.Ws.PutPacket(json.ToUtf8Buffer());
				}

				// var player = (Player)GetNode("%Player");
				// _playerData.x = player.Position.X;
				// _playerData.y = player.Position.Y;
				// var json = JsonConvert.SerializeObject(_playerData);
				// _ws.PutPacket(json.ToUtf8Buffer());

				// var message = System.Text.Encoding.Default.GetString(_ws.GetPacket());
				// var payload = JsonConvert.DeserializeObject<PlayerData[]>(message);

				// foreach (var enemy in _enemies) enemy.QueueFree();
				// _enemies = new();

				// foreach (var data in payload) {
				// 	if (data.id != _playerData.id) {
				// 		var e = (CharacterBody2D)_enemy.Instantiate();
				// 		e.Position = new Vector2(data.x, data.y);
				// 		_enemies.Add(e);
				// 		AddChild(e);
				// 	}
				// }
				
				break;
			case WebSocketPeer.State.Closing:
				GD.Print("Disconnecting from server...");
				break;
			case WebSocketPeer.State.Closed:
				GD.Print("Not connected to server.");
				break;
		}
	}

	public override void _ExitTree() {
		GameInfo.Ws.Close();
			// reason: _playerData.id.ToString()
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


