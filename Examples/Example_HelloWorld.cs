using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleGameEngine;

namespace ConsoleGameEngineExamples {
	class HelloWorld : ConsoleGame {
		static void Main(string[] args) {
			new HelloWorld().Construct(128, 64, 5, 5, FramerateMode.Unlimited);
		}

		Random rand = new Random();
		Point p = new Point(16, 13);

		float a = 0;

		public override void Create() {
			Engine.SetPalette(Palettes.Pico8);
			Engine.SetBackground(0);

			Engine.Borderless(true);
			Console.Title = "Demo";
		}

		public override void Update() {
			p.Y = 13 + (int)(Math.Sin(a*3) * 2.5f);

			a += DeltaTime;
		}

		public override void Render() {
			Engine.ClearBuffer();

			Engine.WriteText(p + new Point(1, 1), "Dungeon", FigletFont.Load("D:\\Game Developement\\Fonts\\FIGlets\\caligraphy.flf"), 1);
			Engine.WriteText(p, "Dungeon", FigletFont.Load("D:\\Game Developement\\Fonts\\FIGlets\\caligraphy.flf"), 8);

			//Engine.SetPixel(Engine.GetMousePos(), ConsoleCharacter.Full, 8);
			Engine.DisplayBuffer();
		}
	}
}
