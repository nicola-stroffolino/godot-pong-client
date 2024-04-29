using Godot;

public partial class Game : Node2D {
	private string _socketUrl = "ws://localhost:5000";
	private readonly WebSocketPeer _ws = new();
	private readonly PlayerData _data = new();

	public override void _Ready(){
		_data.id = 1;

		_ws.Connect("connection_established", new Callable(this, MethodName.OnConnectionEstablished));
		_ws.Connect("connection_error", new Callable(this, MethodName.OnConnectionError));
		_ws.Connect("connection_closed", new Callable(this, MethodName.OnConnectionClosed));
		_ws.Connect("data_received", new Callable(this, MethodName.OnDataReceived));

		var err = _ws.ConnectToUrl(_socketUrl);
		if (err != Error.Ok) GD.Print("Connection Refused");
	}

	private void OnConnectionEstablished() {
		GD.Print("Connected to Server");
	}

	private void OnConnectionError() {
		GD.Print("Connection Error");
	}

	private void OnConnectionClosed() {
		GD.Print("Connection Closed");
	}

	private void OnDataReceived() {
		
	}

	public override void _Process(double delta){
		
	}

	public class PlayerData {
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
