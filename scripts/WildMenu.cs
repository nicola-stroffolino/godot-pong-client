using Godot;

public partial class WildMenu : Control {
	[Signal] public delegate void ColorChoosenEventHandler(CardColor value);
	public CardColor ChoosenColor { get; set; } = CardColor.Wild;

	public override void _Ready() {
		foreach (var node in GetChildren()) {
			if (node is not WildMenuButton btn) continue;

			btn.Connect(WildMenuButton.SignalName.ColorButtonPressed, new Callable(this, MethodName.SetChoosenColor));
		}
	}

	public void SetChoosenColor(CardColor color) {
		EmitSignal(SignalName.ColorChoosen, (int)color);
	}
}
