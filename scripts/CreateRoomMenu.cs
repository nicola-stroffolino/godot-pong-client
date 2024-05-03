using Godot;
using System;

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
		
		var err = GameInfo.Ws.ConnectToUrl(GameInfo.SocketUrl);
		if (err != Error.Ok) {
			GD.Print("Connection Refused");
			return;
		}
		
		GameInfo.Queue.Enqueue(createRoom);
	}

	public void OnBackButtonPressed() {
		GetTree().ChangeSceneToPacked(Scenes.StartMenu);
	}
}
