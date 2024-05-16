using Godot;
using System;

public partial class EndScreen : Control {
	private Button _exitBtn;
	private Label _outcomeLbl;
	private AnimationPlayer _animPlayer;

	public override void _Ready() {
		_exitBtn = GetNode<Button>("%ExitButton");
		_outcomeLbl = GetNode<Label>("%Outcome");
		_animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

		_exitBtn.Connect(Button.SignalName.Pressed, Callable.From(ReturnToMainMenu));
		_animPlayer.Play("spawn");
	}

	public void ReturnToMainMenu() {
		GameInfo.Ws.Close();
		GetTree().ChangeSceneToPacked(Scenes.StartMenu);
	}

	public void SetOutcome(bool winner) {
		if (winner) _outcomeLbl.Text = "You Won!";
		else _outcomeLbl.Text = "You Lost.";
	} 
}
