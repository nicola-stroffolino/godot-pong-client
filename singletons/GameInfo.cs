using System.Collections.Generic;
using Godot;

public partial class GameInfo : Node {
    public static string SocketUrl { get; } = "wss://godot-uno.glitch.me";
	public static WebSocketPeer Ws { get; } = new();
    public static Queue<MyDTO> Queue { get; } = new();
    public static Card PlayedCard { get; set; }
    public static int DeckCardsNumber { get; set; } = 0;
}