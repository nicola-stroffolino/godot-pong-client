using Godot;

public partial class Background : ParallaxBackground {
    [Export]
    public float ScrollSpeed { get; set; } = 100.0f;

	public override void _Process(double delta) {
        Vector2 offset = new(ScrollSpeed * (float)delta, 0);
        ScrollOffset -= offset;
    }
}
