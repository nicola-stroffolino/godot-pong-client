using Godot;
using System;

public partial class Player : CharacterBody2D {
	private const float SPEED = 300.0f;
	private Vector2 _direction = Vector2.Zero;
	private Vector2 _velocity = Vector2.Zero;

	public override void _PhysicsProcess(double delta) {
		if (_direction != Vector2.Zero) {
			_velocity.X = _direction.X * SPEED;
			_velocity.Y = _direction.Y * SPEED;
		} else {
			_velocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);
			_velocity.Y = Mathf.MoveToward(Velocity.Y, 0, SPEED);
		}

		Velocity = _velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event) {
		_velocity = Velocity;
		_direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
	}
}
