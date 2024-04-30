using Newtonsoft.Json;
using Godot;
using System;
using Godot.Collections;

public partial class Game : Node2D {
	private string _socketUrl = "ws://localhost:5000";
	private readonly WebSocketPeer _ws = new();
	private readonly PlayerData _playerData = new();

	private PackedScene _enemy = (PackedScene)GD.Load("res://scenes/Enemy.tscm");
	private Array<CharacterBody2D> _enemies = new();

	public override void _Ready(){
		_playerData.id = 1;

		var err = _ws.ConnectToUrl(_socketUrl);
		if (err != Error.Ok) GD.Print("Connection Refused");
	}

	public override void _PhysicsProcess(double delta) {
		_ws.Poll();
		var state = _ws.GetReadyState();
		switch (state) {
			case WebSocketPeer.State.Connecting:
				GD.Print("Connecting to server...");
				break;
			case WebSocketPeer.State.Open:
				// GD.Print("Connected to server.");

				var player = (Player)GetNode("%Player");
				_playerData.x = player.Position.X;
				_playerData.y = player.Position.Y;
				var json = JsonConvert.SerializeObject(_playerData);
				_ws.PutPacket(json.ToUtf8Buffer());

				var message = System.Text.Encoding.Default.GetString(_ws.GetPacket());
				var payload = JsonConvert.DeserializeObject<PlayerData[]>(message);

				foreach (var enemy in _enemies) enemy.QueueFree();
				_enemies = new();

				foreach (var data in payload) {
					if (data.id != _playerData.id) {
						
					}
				}

				// 	for enemy in enemies:
				// 		enemy.queue_free()
				// 	enemies = []
				// 	for player in payload:
				// 		if player.id != data["id"]:
				// 			var e = enemy.instance()
				// 			e.position = Vector2(player["x"], player["y"])
				// 			enemies.append(e)
				// 			add_child(e)

				
				break;
			case WebSocketPeer.State.Closing:
				GD.Print("Disconnecting from server...");
				break;
			case WebSocketPeer.State.Closed:
				GD.Print("Disconnected from server.");
				break;
		}
	}

	public override void _ExitTree() {
		_ws.Close(reason: _playerData.id.ToString());
	}

	class PlayerData {
		public float x { get; set; } = 0;
		public float y { get; set; } = 0;
		public int id { get; set; } = 0;
	}

	// func _ready():
	// #	data["id"] = $Player.id
	// 	rng.randomize()
	// 	data["id"] = rng.randi_range(1, 100)
		
	// 	ws.connect('connection_closed', self, '_closed')
	// 	ws.connect('connection_error', self, '_closed')
	// 	ws.connect('connection_established', self, '_connected')
	// 	ws.connect('data_received', self, '_on_playerData')
		
	// 	var customHeaders = PoolStringArray(["user-agent: Godot"]) 
	// 	var err = ws.connect_to_url(URL, PoolStringArray(), false, customHeaders)
	// 	if err != OK:
	// 		print('connection refused')

	// func _closed():
	// 	print("connection closed")
		
	// func _connected():
	// 	print("connected to host")
		
	// func _on_playerData():
	// 	var payload = JSON.parse(ws.get_peer(1).get_packet().get_string_from_utf8()).result
	// 	for enemy in enemies:
	// 		enemy.queue_free()
	// 	enemies = []
	// 	for player in payload:
	// 		if player.id != data["id"]:
	// 			var e = enemy.instance()
	// 			e.position = Vector2(player["x"], player["y"])
	// 			enemies.append(e)
	// 			add_child(e)

	// func _process(delta):
	// 	data["x"] = $Player.position.x
	// 	data["y"] = $Player.position.y
	// 	ws.get_peer(1).put_packet(JSON.print(data).to_utf8())
	// 	ws.poll()

	// func _on_Button_pressed():
	// 	ws.disconnect_from_host(1000, str(data["id"]))
	// 	get_tree().quit()
}
