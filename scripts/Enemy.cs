using Godot;
using System;

public partial class Enemy : CharacterBody2D {
	public override void _Ready() {
		Modulate = new Color(1, 0, 0);
	}
}
