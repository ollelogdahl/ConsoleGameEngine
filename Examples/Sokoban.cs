using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleGameEngine;

namespace ConsoleGameEngineExamples {
	class Sokoban : ConsoleGame {
		private static void Main(string[] args) {
			new Sokoban().Construct(16, 16, 16, 16, FramerateMode.Unlimited);
		}

		int[,] map;
		Point player = new Point(1, 2);

		public override void Create() {
			Engine.SetPalette(Palettes.Pico8);
			Engine.Borderless();

			map = new int[10, 8] {
				{ 0, 0, 0, 1, 1, 1, 1, 1},
				{ 1, 1, 1, 1, 0, 0, 0, 1},
				{ 1, 0, 3, 0, 0, 1, 0, 1},
				{ 1, 0, 3, 3, 3, 0, 0, 1},
				{ 1, 1, 4, 1, 4, 0, 0, 1},
				{ 1, 0, 0, 4, 4, 1, 0, 1},
				{ 1, 0, 0, 1, 4, 3, 0, 1},
				{ 1, 0, 3, 4, 3, 0, 1, 1},
				{ 1, 1, 1, 0, 0, 0, 1, 0},
				{ 0, 0, 1, 1, 1, 1, 1, 0},
			};



		}

		public override void Update() {
			if(Engine.GetKeyDown(ConsoleKey.UpArrow)) {
				
			}
		}

		public override void Render() {
			Engine.ClearBuffer();

			Engine.Frame(new Point(1, 1), new Point(14, 14), 7);

			Point offset = new Point(4, 3);

			for (int y = 0; y < map.GetLength(0); y++) {
				for (int x = 0; x < map.GetLength(1); x++) {
					Point p = new Point(x, y) + offset;

					switch (map[y, x]) {
						case 1:
							Engine.SetPixel(p, 7, ConsoleCharacter.Full);
							break;
						case 3:
							Engine.SetPixel(p, 9, ConsoleCharacter.Full);
							break;
						case 4:
							Engine.SetPixel(p, 8, (ConsoleCharacter)'x');
							break;
					}
				}
			}

			Engine.SetPixel(player + offset, 8, (ConsoleCharacter)'@');

			Engine.DisplayBuffer();
		}
	}
}
