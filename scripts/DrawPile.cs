using System;
using Godot;

public partial class DrawPile : Marker2D {
    private Control _pile;
    private Button _drawBtn;
    private Card _card;
    private Node2D _pointer;

    public override void _Ready() {
        _pile = GetNode<Control>("%Pile");
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
        var difference = GetChildCount() - GameInfo.DeckCardsNumber;
        if (difference < 0) { // add cards 
            for (int i = 0; i < -difference - 1; i++) {
                _pile.AddChild(_card);
                _card.Position -= new Vector2(.05f, .05f);
                _card.ObliterateFromTheFaceOfTheEarth();

                _card = (Card)_card.Duplicate();
            }
        } else { // remove cards
            for (int i = 0; i < difference; i++) {
                _pile.GetChild(_pile.GetChildCount() - 1 - i).QueueFree();
            }
        }
    }
}



