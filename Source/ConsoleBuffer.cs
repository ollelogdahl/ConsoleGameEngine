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

		public void SetBuffer(char[,] charBuffer, int[,] colorBuffer, int background) {
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					int i = (y * width) + x;

					CharInfoBuffer[i].Attributes = (short)(colorBuffer[x, y] |(background << 4) );
					CharInfoBuffer[i].UnicodeChar = charBuffer[x, y];
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
