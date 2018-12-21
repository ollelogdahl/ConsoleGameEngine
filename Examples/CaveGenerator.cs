using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGameEngine;

namespace ConsoleGameEngineExamples {
	class CaveGenerator : ConsoleGame {
		static void Main(string[] args) {
			new CaveGenerator().Construct(size.X, size.Y + 1, 8, 8, FramerateMode.Unlimited);
		}

		static Point size = new Point(96, 64);

		int[,] m;
		Random rand = new Random();

		private int seed;
		private int rfp = 48;
		private int scount = 6;
		private int max = 4;
		private int min = 4;

		int sel = 0;

		public override void Create() {
			Engine.SetPalette(Palettes.Default);
			Engine.Borderless();

			seed = rand.Next(int.MinValue, int.MaxValue);
			m = Generate(size.X, size.Y, rfp, scount, max, min, seed);
		}

		public override void Update() {
			if (Engine.GetKeyDown(ConsoleKey.Spacebar)) {
				seed = rand.Next(int.MinValue, int.MaxValue);
				m = Generate(size.X, size.Y, rfp, scount, max, min, seed);
			}


			if(Engine.GetKeyDown(ConsoleKey.RightArrow) && sel != 3) sel++;
			if(Engine.GetKeyDown(ConsoleKey.LeftArrow) && sel != 0) sel--;

			if(Engine.GetKeyDown(ConsoleKey.UpArrow)) switch(sel) {
					case 0: rfp++; break;
					case 1: scount++; break;
					case 2: max++; break;
					case 3: min++; break;
			}
			if (Engine.GetKeyDown(ConsoleKey.DownArrow)) switch (sel) {
					case 0: rfp--; break;
					case 1: scount--; break;
					case 2: max--; break;
					case 3: min--; break;
				}
		}

		public override void Render() {
			Engine.ClearBuffer();

			for(int i = 0; i < size.X; i++) {
				for(int j = 0; j < size.Y; j++) {
					int col = (m[i, j] == 1) ? 0 : 15;
					Engine.SetPixel(new Point(i, j), col);
				}
			}

			Engine.WriteText(new Point(0, size.Y), $"S: {seed}", 8);
			Engine.WriteText(new Point(15, size.Y), $"W: {size.ToString()}", 8);
			Engine.WriteText(new Point(32, size.Y), $"RFP: {rfp}", 8);
			Engine.WriteText(new Point(41, size.Y), $"S: {scount}", 8);
			Engine.WriteText(new Point(48, size.Y), $"MX: {max}", 8);
			Engine.WriteText(new Point(55, size.Y), $"MN: {min}", 8);

			switch (sel) {
				case 0: Engine.WriteText(new Point(32, size.Y), $"RFP: {rfp}", 12); break;
				case 1: Engine.WriteText(new Point(41, size.Y), $"S: {scount}", 12); break;
				case 2: Engine.WriteText(new Point(48, size.Y), $"MX: {max}", 12); break;
				case 3: Engine.WriteText(new Point(55, size.Y), $"MN: {min}", 12); break;

			}

			Engine.DisplayBuffer();
		}



		public int[,] Generate(int width, int height, int randomFillPercent = 45, int smoothCount = 5, int maxNeighbors = 4, int minNeighbors = 4, int seed = -1) {

			int[,] map = new int[width, height];

			if( seed == -1) seed = rand.Next(int.MinValue, int.MaxValue);
			Random prng = new Random(seed);

			// Generera
			for(int x = 0; x < width; x++) {
				for(int y = 0; y < height; y++) {
					if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
						map[x, y] = 1;
					} else {
						map[x, y] = (prng.Next(0, 100) < randomFillPercent) ? 1 : 0;
					}
				}
			}

			// Smooth
			int[,] smoothMap = new int[width, height];
			for (int i = 0; i < smoothCount; i++) {

				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						int neighbors = CountNeighbors(map, x, y, width, height);

						if (neighbors > maxNeighbors) smoothMap[x, y] = 1;
						else if (neighbors < minNeighbors) smoothMap[x, y] = 0;
					}
				}
				map = smoothMap;
			}

			return map;
		}

		public int CountNeighbors(int[,] map, int gridX, int gridY, int w, int h) {
			int count = (map[gridX, gridY] == 1) ? -1 : 0;		// exkludera center ifall den är en vägg
			for(int x = gridX-1; x <= gridX + 1; x++) {
				for(int y = gridY-1; y <= gridY + 1; y++) {
					if (x < 0 || x >= w || y < 0 || y >= h) { count++; continue; }
					count += map[x, y];
				}
			}

			return count;
		}
	}
}
