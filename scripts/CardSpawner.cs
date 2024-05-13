using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardSpawner : Node2D {
	public List<Card> Hand { get; set; } = new();
	public List<Card> OppHand { get; set; } = new();

	public override void _Ready() {
		// foreach (var resName in DirAccess.GetFilesAt("res://resources/cards")) {
		// 	var newCard = (Card)(GD.Load("res://scenes/card.tscn") as PackedScene).Instantiate();
		// 	newCard.Data = (CardData)GD.Load("res://resources/cards/" + resName);
		// 	CardList[resName.Split('.')[0]] = newCard;
		// }

		// var card = (Card)(GD.Load("res://scenes/card.tscn") as PackedScene).Instantiate();
		// for (int i = 0; i < 9; i++) {
		// 	for (int j = 0; j < 4; j++) {
		// 		NumberCardData newCardData = new() {
		// 			Value = i,
		// 			Color = (CardColor)j
		// 		};
		// 		var newCard = card.CreateCard(newCardData);
				
		// 		CardList[i.ToString() + '_' + j.ToString()] = newCard;	
		// 	}
		// }

		// SpreadCards(0, 200, 100);   
	}

	public override void _Process(double delta) {
		var message = System.Text.Encoding.Default.GetString(GameInfo.Ws.GetPacket());
		var payload = JsonConvert.DeserializeObject<JObject>(message);

		if (payload is not null) {
			if (payload["draw"] is not null && payload["oppDrawNumber"] is not null) {
				GD.Print(payload);
				var drawnCards = payload["draw"].ToObject<string[]>();
				foreach (var card in drawnCards) {
					var cardInfo = card.Split('_');
					var color = cardInfo[0];
					var value = cardInfo[1];

					Card newCard = (Card)(GD.Load("res://scenes/card.tscn") as PackedScene).Instantiate();
					CardData newCardData = new() {
						Value = value,
						Color = (CardColor)Enum.Parse(typeof(CardColor), color)
					};
					newCard = newCard.CreateCard(newCardData);
					newCard.Connect(Card.SignalName.CardClicked, new Callable(this, MethodName.DespawnCard));
					
					Hand.Add(newCard);
				}
				
				var enemyCardsNumber = (int)payload["oppDrawNumber"];
				for (int i = 0; i < enemyCardsNumber; i++) {
					Card newCard = (Card)(GD.Load("res://scenes/card.tscn") as PackedScene).Instantiate();
					CardData newCardData = new() {
						Value = "Back",
						Color = CardColor.Wild
					};
					newCard = newCard.CreateCard(newCardData);
					
					OppHand.Add(newCard);
				}
				
				SpreadCards(-100, 100, 100, (Marker2D)GetNode("%CardSpawn"), Hand);
				SpreadCards(-100, 100, 100, (Marker2D)GetNode("%OppCardSpawn"), OppHand);
			}
		}
	}

	public void SpreadCards(float x1, float x2, float k, Marker2D node, List<Card> cardList) {
		var shift = 0f;
		if (x1 < 0 && x2 > 0) {
			shift = x1;
			x2 -= x1;
			x1 = 0;
		} else if (x1 < 0 && x2 < 0) {
			shift = x2;
			var tmp = x1;
			x1 = -x2;
			x2 = -tmp;
		}

		var r = (x2 - x1) / 2 + k;
		var @base = (x2 - x1) / 2;
		var step = (x2 - x1) / (cardList.Count -1);
		var angle = MathF.Atan2(@base, GetUntranslatedArch(r, @base, x2));

		var cardIdx = 0;
		for (var x = x1; x <= x2; x += step, cardIdx++) {
			var y = GetUntranslatedArch(r, @base, x) - (MathF.Sin(angle) * r);

			var card = cardList.ElementAt(cardIdx);
			card.Position = new Vector2(x + shift, -y);
			var rotation = -Mathf.Atan(Mathf.Sqrt(4 * r * r - Mathf.Pow(2 * x - x2 + x1, 2)) / (2 * x - x2 + x1));

			if (rotation > 0) card.Rotation = rotation - Mathf.Pi / 2;
			else card.Rotation = rotation + Mathf.Pi / 2;
			
			if(!node.GetChildren().Contains(card)) node.AddChild(card);
		}

		static float GetUntranslatedArch(float r, float @base, float x) => Mathf.Sqrt(r * r - (x - @base) * (x - @base));
	}

	public void DespawnCard(Card card, Marker2D parent) {
		Hand.Remove(card);
		card.QueueFree();
		SpreadCards(-100, 100, 100, parent, Hand);
	}
}
