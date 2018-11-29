using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ConsoleGameEngine;

namespace ConsoleGameEngineExamples {
	class Tron : ConsoleGame {

		Color[] palette = new Color[5] {
			new Color(10, 10, 10),
			new Color(255, 0, 0),
			new Color(100, 0, 0),
			new Color(0,   0, 255),
			new Color(0,   0, 100),
		};

		static int gameWidth = 64;
		static int gameHeight = 64;

		Point p1;
		Point v1;
		Point p2;
		Point v2;

		List<Point> p1List = new List<Point>();
		List<Point> p2List = new List<Point>();

		int gamestate = 0;	// 0 - spelar, 1 - spelare1 vann, 2 - spelare 2 vann, 3 - lika

		private static void Main(string[] args) {
			new Tron().Construct(gameWidth, gameHeight, 8, 8, FramerateMode.MaxFps);
		}

		public override void Create() {
			Engine.SetPalette(palette);
			Engine.Borderless(true);
			TargetFramerate = 30;

			p1 = new Point(1, 1);
			p2 = new Point(gameWidth - 2, gameHeight - 2);
			v1 = new Point(0, 1); v2 = new Point(0, -1);
		}

		public override void Update() {
			// tar input
			if (gamestate == 0) {
				// spelare 1
				if (Engine.GetKey(ConsoleKey.W)) v1 = new Point(0, -1);
				if (Engine.GetKey(ConsoleKey.A)) v1 = new Point(-1, 0);
				if (Engine.GetKey(ConsoleKey.S)) v1 = new Point(0, 1);
				if (Engine.GetKey(ConsoleKey.D)) v1 = new Point(1, 0);

				// spelare 2
				if (Engine.GetKey(ConsoleKey.UpArrow)) v2 = new Point(0, -1);
				if (Engine.GetKey(ConsoleKey.LeftArrow)) v2 = new Point(-1, 0);
				if (Engine.GetKey(ConsoleKey.DownArrow)) v2 = new Point(0, 1);
				if (Engine.GetKey(ConsoleKey.RightArrow)) v2 = new Point(1, 0);

				p1List.Add(p1);
				p2List.Add(p2);

				p1 += v1;
				p2 += v2;

				if (p1.X == p2.X && p1.Y == p2.Y) {
					gamestate = 3;
				}

				for (int i = 0; i < p1List.Count; i++) {
					Point a = p1List[i];
					Point b = p2List[i];

					if (p1.X == a.X && p1.Y == a.Y) gamestate = 2;
					if (p1.X == b.X && p1.Y == b.Y) gamestate = 2;

					if (p2.X == a.X && p2.Y == a.Y) gamestate = 1;
					if (p2.X == b.X && p2.Y == b.Y) gamestate = 1;
				}
			}

			if(gamestate == 1) {
				Engine.WriteText(new Point(gameWidth / 2 - 6, gameHeight / 2), "Player 1 won", 1);
				Restart();
			}
			if (gamestate == 2) {
				Engine.WriteText(new Point(gameWidth / 2 - 6, gameHeight / 2), "Player 2 won", 3);
				Restart();
			}

			if(gamestate == 3) {
				Engine.WriteText(new Point(gameWidth / 2 - 2, gameHeight / 2), "Draw", 1);
				Restart();
			}
		}

		public override void Render() {
			Engine.ClearBuffer();

			for(int i = 0; i < p1List.Count; i++) {
				Engine.SetPixel(p1List[i], ConsoleCharacter.Light, 2);
				Engine.SetPixel(p2List[i], ConsoleCharacter.Light, 4);
			}

			Engine.SetPixel(p1, ConsoleCharacter.Full, 1);
			Engine.SetPixel(p2, ConsoleCharacter.Full, 3);

			Engine.DisplayBuffer();
		}

		void Restart() {
			Engine.DisplayBuffer();
			Thread.Sleep(4000);
			gamestate = 0;

			p1 = new Point(1, 1);
			p2 = new Point(gameWidth - 2, gameHeight - 2);
			v1 = new Point(0, 1); v2 = new Point(0, -1);

			p1List.Clear();
			p2List.Clear();
		}
	}
}
