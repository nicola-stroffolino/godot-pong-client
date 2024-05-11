using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class CardSpawner : Node2D {
	public Dictionary<string, Card> CardList { get; set; } = new();
	public override void _Ready() {
		foreach (var resName in DirAccess.GetFilesAt("res://resources/cards")) {
			var newCard = (Card)(GD.Load("res://scenes/card.tscn") as PackedScene).Instantiate();
			newCard.Data = (CardData)GD.Load("res://resources/cards/" + resName);
			CardList[resName.Split('.')[0]] = newCard;
			// AddChild(newCard);
		}

		SpreadCards(10, -45, 45);
	}

	public override void _Process(double delta) {

	}

	public void SpreadCards(float radius, float startAngle, float endAngle) {
        int numCards = CardList.Count;
        
        // Calculate the angle step between each card
        float angleStep = (endAngle - startAngle) / (numCards - 1);

        // Iterate through each card and position it on the arch
        for (int i = 0; i < numCards; i++)
        {
            // Calculate the angle for this card
            float angle = startAngle + i * angleStep;

            // Convert angle to radians
            float radians = Mathf.DegToRad(angle);

            // Calculate the position on the arch using trigonometry
            float x = radius * Mathf.Cos(radians);
            float y = radius * Mathf.Sin(radians);

			// Set the position of the card
			var card = CardList.ElementAt(i).Value;
            card.Position = new Vector2(x, y);
			AddChild(card);
        }
    }
}
