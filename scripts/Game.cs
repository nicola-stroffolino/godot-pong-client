using Newtonsoft.Json;
using Godot;

public partial class Game : Node2D {
	private string _socketUrl = "ws://localhost:5000";
	private readonly WebSocketPeer _ws = new();
	private readonly PlayerData _data = new();

	public override void _Ready(){
		_data.id = 1;

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
				GD.Print("Connected to server.");

				var player = (Player)GetNode("%Player");
				_data.x = player.Position.X;
				_data.y = player.Position.Y;
				var json = JsonConvert.SerializeObject(_data);
				_ws.PutPacket(json.ToUtf8Buffer());
				
				break;
			case WebSocketPeer.State.Closing:
				GD.Print("Disconnecting from server...");
				break;
			case WebSocketPeer.State.Closed:
				GD.Print("Disconnected from server.");
				break;
		}

		// // Handle incoming data
		// var data = _ws.GetPacket();
		// if (data != null)
		// {
		//     string message = data.ToString();
		//     // Process the received message (similar to OnDataReceived)
		//     var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
		//     string action = dict["action"] as string;
		//     // Handle message actions as before
		// }
	}

	public override void _ExitTree() {
		_ws.Close(reason: "quit");
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
	// 	ws.connect('data_received', self, '_on_data')
		
	// 	var customHeaders = PoolStringArray(["user-agent: Godot"]) 
	// 	var err = ws.connect_to_url(URL, PoolStringArray(), false, customHeaders)
	// 	if err != OK:
	// 		print('connection refused')

	// func _closed():
	// 	print("connection closed")
		
	// func _connected():
	// 	print("connected to host")
		
	// func _on_data():
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
