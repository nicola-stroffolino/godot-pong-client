using Godot;
using System;

public partial class StartMenu : CenterContainer {
	private Button _createMenuBtn;
	private Button _joinMenuBtn;
	public override void _Ready() {
		_createMenuBtn = GetNode<Button>("%CreateMenuButton");
		_joinMenuBtn = GetNode<Button>("%JoinMenuButton");

		_createMenuBtn.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.SwitchToCreateMenu));
		_joinMenuBtn.Connect(Button.SignalName.Pressed, new Callable(this, MethodName.SwitchToJoinMenu));
	}

	public override void _Process(double delta) {
		GameInfo.Ws.Poll();
	}

	public void SwitchToCreateMenu() {
		GetTree().ChangeSceneToPacked(Scenes.CreateMenu);
	}

	public void SwitchToJoinMenu() {
		GetTree().ChangeSceneToPacked(Scenes.JoinMenu);
	}
}
