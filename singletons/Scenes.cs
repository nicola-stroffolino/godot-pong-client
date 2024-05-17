using Godot;
using System;

public partial class Scenes : Node {
	public static PackedScene StartMenu { get; } = (PackedScene)GD.Load("res://scenes/start_menu.tscn");
	public static PackedScene CreateMenu { get; } = (PackedScene)GD.Load("res://scenes/create_room_menu.tscn");
	public static PackedScene JoinMenu { get; } = (PackedScene)GD.Load("res://scenes/join_room_menu.tscn");
	public static PackedScene Game { get; } = (PackedScene)GD.Load("res://scenes/game.tscn");
	public static PackedScene Card { get; } = (PackedScene)GD.Load("res://scenes/card.tscn");
	public static PackedScene Pointer { get; } = (PackedScene)GD.Load("res://scenes/pointer.tscn");
	public static PackedScene EndScreen { get; } = (PackedScene)GD.Load("res://scenes/end_screen.tscn");
	public static PackedScene WildMenu { get; } = (PackedScene)GD.Load("res://scenes/wild_menu.tscn");
}
