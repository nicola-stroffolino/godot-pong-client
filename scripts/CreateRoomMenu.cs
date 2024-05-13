using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Text.Json;

public partial class CreateRoomMenu : CenterContainer {
	private LineEdit _nameInput;
	private LineEdit _passwordInput;
	private LineEdit _nicknameInput;
	private Button _createBtn;
	private Button _backBtn;

	public override void _Ready() {
		_nameInput = GetNode<LineEdit>("%NameInput");
		_passwordInput = GetNode<LineEdit>("%PasswordInput");
		_nicknameInput = GetNode<LineEdit>("%NicknameInput");
		_createBtn = GetNode<Button>("%CreateButton");
		_backBtn = GetNode<Button>("%BackButton");

		_createBtn.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.OnCreateButtonPressed));
		_backBtn.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.OnBackButtonPressed));
	}

	public void OnCreateButtonPressed() {
		var name = _nameInput.Text;
		var pass = _passwordInput.Text;
		var nick = _nicknameInput.Text;
		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(nick)) return;

		MyDTO createRoom = new() {
			RequestType = "Create Room",
			Data = new {
				Name = name,
				Password = pass,
				Nickname = nick
			}
		};
		
		if (GameInfo.Ws.GetReadyState() == WebSocketPeer.State.Closed) {
			GameInfo.Ws.HandshakeHeaders = new string[] { "user-agent: Godot" };
			var err = GameInfo.Ws.ConnectToUrl(GameInfo.SocketUrl);
			if (err != Error.Ok) {
				GD.Print("Connection Refused");
				return;
			}
		}
		
		GameInfo.Queue.Enqueue(createRoom);
	}

	public void OnBackButtonPressed() {
		GetTree().ChangeSceneToPacked(Scenes.StartMenu);
	}

	public override void _Process(double delta) {
		GameInfo.Ws.Poll();
		if (GameInfo.Ws.GetReadyState() == WebSocketPeer.State.Open) {
			if(GameInfo.Queue.Count != 0) {
				var el = GameInfo.Queue.Dequeue();
				var json = JsonConvert.SerializeObject(el);
				GameInfo.Ws.PutPacket(json.ToUtf8Buffer());
			}

			var message = System.Text.Encoding.Default.GetString(GameInfo.Ws.GetPacket());
			var payload = JsonConvert.DeserializeObject<JObject>(message);

			if (payload is null) return;

			if (payload["error"] != null) {
				GD.Print(payload["error"]);
			} else if (payload["room"] != null && payload["player"] != null) {
				PlayerInfo.ConnectedRoomName = (string)payload["room"]["name"];
				PlayerInfo.Id = (int)payload["player"]["id"];
				PlayerInfo.Nickname = (string)payload["player"]["nickname"];
				PlayerInfo.IsYourTurn = true;

				GetTree().ChangeSceneToPacked(Scenes.Game);
			}
		}
	}
}
