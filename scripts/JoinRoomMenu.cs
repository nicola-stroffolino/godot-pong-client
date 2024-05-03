using Godot;
using System;

public partial class JoinRoomMenu : CenterContainer {
	private LineEdit _nameInput;
	private LineEdit _passwordInput;
	private LineEdit _nicknameInput;
	private Button _joinBtn;
	private Button _backBtn;

    public override void _Ready() {
		_nameInput = GetNode<LineEdit>("%NameInput");
		_passwordInput = GetNode<LineEdit>("%PasswordInput");
		_nicknameInput = GetNode<LineEdit>("%NicknameInput");
		_joinBtn = GetNode<Button>("%JoinButton");
		_backBtn = GetNode<Button>("%BackButton");

		_joinBtn.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.OnJoinButtonPressed));
		_backBtn.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.OnBackButtonPressed));
    }

	public void OnJoinButtonPressed() {

	}

	public void OnBackButtonPressed() {
		GetTree().ChangeSceneToPacked(Scenes.StartMenu);
	}
}
