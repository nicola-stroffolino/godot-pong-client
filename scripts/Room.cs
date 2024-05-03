public class Room {
    public string Name { get; set; }
    public string Password { get; set; }
    public int MaxPlayers { get; set; } = 2;
    public int ConnectedPlayers { get; set; }
}