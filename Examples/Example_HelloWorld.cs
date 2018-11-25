using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleGameEngine;

namespace ConsoleGameEngineExamples {
	class HelloWorld : ConsoleGame {
		static void Main(string[] args) {
			new HelloWorld().Construct(128, 64, 8, 8, FramerateMode.Unlimited);
		}

		Random rand = new Random();

		List<Point> points = new List<Point>();

		public override void Create() {
			Engine.SetPalette(Palettes.Pico8);
			Engine.SetBackground(0);

			Engine.Borderless(true);
			Console.Title = "Demo";

			points.Add(new Point(16, 16));
		}

		public override void Update() {
			if(Engine.GetMouseLeft()) {
				points.Add(Engine.GetMousePos());
			}
		}

		public override void Render() {
			Engine.ClearBuffer();

			for(int i = 0; i < points.Count-1; i++) {
				Engine.Line(points[i], points[i + 1], 7, ConsoleCharacter.Full);
				Engine.SetPixel(points[i], ConsoleCharacter.Full, 8);
			}
			Engine.Line(points.Last(), Engine.GetMousePos(), 7, ConsoleCharacter.Full);
			Engine.SetPixel(points.Last(), ConsoleCharacter.Full, 8);

			Engine.SetPixel(Engine.GetMousePos(), ConsoleCharacter.Full, 8);
			Engine.DisplayBuffer();
		}
	}
}
