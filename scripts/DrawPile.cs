using Godot;

public partial class DrawPile : Marker2D {
    public Button _drawPileBtn;
    public Node2D _pointer;

    public override void _Ready() {
        _drawPileBtn = GetNode<Button>("Back");

        _drawPileBtn.Disabled = true;
        _drawPileBtn.Connect(Button.SignalName.Pressed, Callable.From(OnDrawPileClicked));
    }

    private void OnDrawPileClicked() {
		MyDTO drawCard = new() {
            RequestType = "Draw Card",
            Data = new {
                Quantity = 1
            }
        };

        GameInfo.Queue.Enqueue(drawCard);
        
        _drawPileBtn.Disabled = true;
        RemoveChild(_pointer);
        _pointer.QueueFree();
	}

    public void UnlockDrawFeature() {
        _drawPileBtn.Disabled = false;
        _pointer = (Node2D)Scenes.Pointer.Instantiate();
        AddChild(_pointer);
    }
}



