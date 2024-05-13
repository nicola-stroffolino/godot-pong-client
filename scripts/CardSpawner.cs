using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardSpawner : Node2D {
	public Marker2D CardSpawn { get; set; }
	public Marker2D OppCardSpawn { get; set; }
	public Marker2D PlayedCardSpawn { get; set; }
	private Tween _tween;

	public override void _Ready() {
		CardSpawn = (Marker2D)GetNode("%CardSpawn");
		OppCardSpawn = (Marker2D)GetNode("%OppCardSpawn");
		PlayedCardSpawn = (Marker2D)GetNode("%PlayedCardSpawn");
	}

	public override void _Process(double delta) {
		var message = System.Text.Encoding.Default.GetString(GameInfo.Ws.GetPacket());
		var payload = JsonConvert.DeserializeObject<JObject>(message);

		if (payload is not null) {
			if (payload["draw"] is not null && payload["oppDrawNumber"] is not null && payload["playedCard"] is not null) {
				GD.Print(payload);
				PackedScene cardScn = GD.Load("res://scenes/card.tscn") as PackedScene;
				
				var drawnCards = payload["draw"].ToObject<string[]>();
				foreach (var cardName in drawnCards) {
					var newCard = (Card)cardScn.Instantiate();
					newCard.CreateCard(cardName);
					newCard.Connect(Card.SignalName.CardClicked, new Callable(this, MethodName.DespawnCard));
					
					CardSpawn.AddChild(newCard);
				}
				
				var enemyCardsNumber = (int)payload["oppDrawNumber"];
				for (int i = 0; i < enemyCardsNumber; i++) {
					var newCard = (Card)cardScn.Instantiate();
					newCard.CreateCard("Wild_Back");
					
					OppCardSpawn.AddChild(newCard);
				}

				var playedCard = (Card)cardScn.Instantiate();
				playedCard.CreateCard((string)payload["playedCard"]);
				GameInfo.PlayedCard = playedCard;
				PlayedCardSpawn.AddChild(playedCard);
				
				SpreadCards(-100, 100, 100, CardSpawn);
				SpreadCards(-100, 100, 100, OppCardSpawn);
			} else if (payload["newColor"] is not null && payload["newValue"] is not null) {
				PackedScene cardScn = GD.Load("res://scenes/card.tscn") as PackedScene;
				GameInfo.PlayedCard = (Card)cardScn.Instantiate();
				GameInfo.PlayedCard.CreateCard($"{payload["newColor"]}_{payload["newValue"]}");
				
				PlayedCardSpawn.AddChild(GameInfo.PlayedCard);
			}
		}
	}

	public void SpreadCards(float x1, float x2, float k, Marker2D node) {
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
		var step = (x2 - x1) / (node.GetChildCount() - 1);
		var angle = MathF.Atan2(@base, GetUntranslatedArch(r, @base, x2));

		var cardIdx = 0;
		for (var x = x1; x <= x2; x += step, cardIdx++) {
			var y = GetUntranslatedArch(r, @base, x) - (MathF.Sin(angle) * r);

			var card = (Card)node.GetChild(cardIdx);
			card.Position = new Vector2(x + shift, -y);
			var rotation = -Mathf.Atan(Mathf.Sqrt(4 * r * r - Mathf.Pow(2 * x - x2 + x1, 2)) / (2 * x - x2 + x1));

			if (rotation > 0) card.Rotation = rotation - Mathf.Pi / 2;
			else card.Rotation = rotation + Mathf.Pi / 2;
			
			if(!node.GetChildren().Contains(card)) node.AddChild(card);
		}

		static float GetUntranslatedArch(float r, float @base, float x) => Mathf.Sqrt(r * r - (x - @base) * (x - @base));
	}

	public void DespawnCard(Card card, Marker2D parent) {
		card.QueueFree();
		SpreadCards(-100, 100, 100, parent);
	}

	public void DrawCards(Vector2 fromPos, int number) {
		if (_tween is not null && _tween.IsRunning()) _tween.Kill();

		_tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
		for (int i = 0; i < number; i++) {
			// to be implemented
		}
	}
}
