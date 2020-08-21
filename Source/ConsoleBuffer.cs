namespace ConsoleGameEngine {
	using System;
	using System.IO;
	using Microsoft.Win32.SafeHandles;

	class ConsoleBuffer {
		private NativeMethods.CharInfo[] CharInfoBuffer { get; set; }
		SafeFileHandle h;

		readonly int width, height;

		public ConsoleBuffer(int w, int he) {
			width = w;
			height = he;

			h = NativeMethods.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

			if (!h.IsInvalid) {
				CharInfoBuffer = new NativeMethods.CharInfo[width * height];
			}
		}
		/// <summary>
		/// Sets the buffer to values
		/// </summary>
		/// <param name="GlyphBuffer"></param>
		/// <param name="charBuffer"> array of chars which get added to the buffer</param>
		/// <param name="colorBuffer"> array of foreground(front)colors which get added to the buffer</param>
		/// <param name="background"> array of background colors which get added to the buffer</param>
		/// <param name="defualtBackground"> default color(may reduce fps?), this is the background color
		///									null chars will get set to this default background</param>
		public void SetBuffer(Glyph[,] GlyphBuffer, int defualtBackground) {
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					int i = (y * width) + x;

					if(GlyphBuffer[x, y].c == 0)
						GlyphBuffer[x, y].bg = defualtBackground;

					CharInfoBuffer[i].Attributes = (short)(GlyphBuffer[x, y].fg |(GlyphBuffer[x,y].bg << 4) );
					CharInfoBuffer[i].UnicodeChar = GlyphBuffer[x, y].c;
				}
			}
		}

		public bool Blit() {
			NativeMethods.SmallRect rect = new NativeMethods.SmallRect() { Left = 0, Top = 0, Right = (short)width, Bottom = (short)height };

			return NativeMethods.WriteConsoleOutputW(h, CharInfoBuffer,
				new NativeMethods.Coord() { X = (short)width, Y = (short)height },
				new NativeMethods.Coord() { X = 0, Y = 0 }, ref rect);
		}
	}
}
