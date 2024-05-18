using Godot;

public partial class WildMenu : Control {
	[Signal] public delegate void ColorChoosenEventHandler(CardColor value);
	private AnimationPlayer _animPlayer;

	public override void _Ready() {
		_animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		_animPlayer.Play("spawn");
		foreach (var node in GetNode<GridContainer>("%GridContainer").GetChildren()) {
			if (node is not WildMenuButton btn) continue;

			btn.Connect(WildMenuButton.SignalName.ColorButtonPressed, new Callable(this, MethodName.SetChoosenColor));
		}
	}

	public void SetChoosenColor(CardColor color) {
		EmitSignal(SignalName.ColorChoosen, (int)color);
		QueueFree();
	}
}
