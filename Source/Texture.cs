using System;
using System.Collections.Generic;
using System.Text;

using System.IO;


namespace ConsoleGameEngine {
	public struct Texture {
		public int[,] ColorTexture { get; set; }
		public ConsoleCharacter[,] CharTexture { get; set; }

		public Texture(Texture spritesheet, Point pointOnSheet, Point size) {
			ColorTexture = new int[size.X, size.Y];
			CharTexture = new ConsoleCharacter[size.X, size.Y];

			for(int x = 0; x < size.X; x++) {
				for(int y = 0; y < size.Y; y++) {
					Point p = new Point(x + pointOnSheet.X, y + pointOnSheet.Y);

					ColorTexture[x, y] = spritesheet.ColorTexture[p.X, p.Y];
					CharTexture[x, y] = spritesheet.CharTexture[p.X, p.Y];
				}
			}
		}

		public Texture(string path) {
			ColorTexture = new int[8, 8];
			CharTexture = new ConsoleCharacter[8, 8];
		}
	}
}
