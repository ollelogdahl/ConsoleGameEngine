using ConsoleGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleGameEngineExamples {

	internal class HelloWorld : ConsoleGame {
		private static void Main(string[] args) {
			new HelloWorld().Construct(128, 64, 4, 4, FramerateMode.MaxFps);
		}

		Point p = new Point(32, 14);
		int i = 0;
		FigletFont font;

		public override void Create() {
			Engine.SetPalette(Palettes.Pico8);
			Engine.Borderless(true);

			TargetFramerate = 15;

			font = FigletFont.Load("caligraphy.flf");
		}

		public override void Update() {
			p.Y = 14 + (int)(Math.Sin(i * 0.1f) * 4f);
			i++;
		}

		public override void Render() {
			Engine.ClearBuffer();

			Engine.WriteFiglet(p, "Hello", font, 8);

			Engine.DisplayBuffer();
		}
	}
}