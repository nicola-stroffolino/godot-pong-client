using System;
using Godot;

public partial class DrawPile : Marker2D {
    private Button _drawBtn;
    private Card _card;
    private Node2D _pointer;
    private int _cycle = 100;

    public override void _Ready() {
        _drawBtn = GetNode<Button>("%DrawButton");
        _card = (Card)Scenes.Card.Instantiate();
        _card.CreateCard("Wild_Back");
        _card.Position = -(_card.Size / 2);

        _drawBtn.Disabled = true;
        _drawBtn.Connect(Button.SignalName.Pressed, Callable.From(OnDrawPileClicked));
    }

    private void OnDrawPileClicked() {
		MyDTO drawCard = new() {
            RequestType = "Draw Card",
            Data = new {
                Quantity = 1
            }
        };

        GameInfo.Queue.Enqueue(drawCard);
        
        _drawBtn.Disabled = true;
        RemoveChild(_pointer);
        _pointer.QueueFree();
	}

    public void UnlockDrawFeature() {
        _drawBtn.Disabled = false;
        _pointer = (Node2D)Scenes.Pointer.Instantiate();
        AddChild(_pointer);
    }

    public void UpdateCardsNumber() {
        if (GameInfo.DeckCardsNumber <= _cycle) {
            _cycle -= 20;
            if (_cycle < 0) _cycle = 0; // or 100 when deck actually cycles

            var pileTexture = (AtlasTexture)GD.Load("res://resources/piles/" + _cycle + ".tres");
            _drawBtn.Icon = pileTexture;
        }
    }
}



