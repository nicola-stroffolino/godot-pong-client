using System.Collections.Generic;
using Godot;

public partial class GameInfo : Node {
    public static string SocketUrl { get; } = "ws://localhost:5000";
	public static WebSocketPeer Ws { get; } = new();
    public static Queue<MyDTO> Queue { get; } = new();
}