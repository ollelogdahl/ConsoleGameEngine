using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleGameEngine;

namespace ConsoleGameEngineExamples {
	class HelloWorld : ConsoleGame {
		static void Main(string[] args) {
			new HelloWorld().Construct(128, 128, 2, 2, FramerateMode.Unlimited);
		}

		Random rand = new Random();

		Point p = new Point(64, 64);
		int close = 0;

		public override void Create() {
			Engine.SetPalette(Palettes.Pico8);
			Engine.SetBackground(0);

			Engine.Borderless(true);
			Console.Title = "Demo";
		}

		public override void Update() {

		}

		public override void Render() {
			Engine.ClearBuffer();

			Engine.FillTriangle(p, p + new Point(10, 10), p + new Point(-10, 10), 8, ConsoleCharacter.Medium);
			Engine.Triangle(p, p + new Point(10, 10), p + new Point(-10, 10), 2, ConsoleCharacter.Full);

			Engine.SetPixel(Engine.GetMousePos(), ConsoleCharacter.Full, 8);
			Engine.DisplayBuffer();
		}
	}
}
