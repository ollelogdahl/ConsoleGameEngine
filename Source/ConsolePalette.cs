namespace ConsoleGameEngine {
	using System;
	using System.Runtime.InteropServices;

	class ConsolePalette {

		public static int SetColor(int consoleColor, Color targetColor) {
			return SetColor(consoleColor, targetColor.R, targetColor.G, targetColor.B);
		}

		private static int SetColor(int color, uint r, uint g, uint b) {
			NativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new NativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX();
			csbe.cbSize = Marshal.SizeOf(csbe);
			IntPtr hConsoleOutput = NativeMethods.GetStdHandle(-11);
			if (hConsoleOutput == new IntPtr(-1)) {
				return Marshal.GetLastWin32Error();
			}
			bool brc = NativeMethods.GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
			if (!brc) {
				return Marshal.GetLastWin32Error();
			}

			switch (color) {
				case 0:
					csbe.black = new NativeMethods.ColorRef(r, g, b);
					break;
				case 1:
					csbe.darkBlue = new NativeMethods.ColorRef(r, g, b);
					break;
				case 2:
					csbe.darkGreen = new NativeMethods.ColorRef(r, g, b);
					break;
				case 3:
					csbe.darkCyan = new NativeMethods.ColorRef(r, g, b);
					break;
				case 4:
					csbe.darkRed = new NativeMethods.ColorRef(r, g, b);
					break;
				case 5:
					csbe.darkMagenta = new NativeMethods.ColorRef(r, g, b);
					break;
				case 6:
					csbe.darkYellow = new NativeMethods.ColorRef(r, g, b);
					break;
				case 7:
					csbe.gray = new NativeMethods.ColorRef(r, g, b);
					break;
				case 8:
					csbe.darkGray = new NativeMethods.ColorRef(r, g, b);
					break;
				case 9:
					csbe.blue = new NativeMethods.ColorRef(r, g, b);
					break;
				case 10:
					csbe.green = new NativeMethods.ColorRef(r, g, b);
					break;
				case 11:
					csbe.cyan = new NativeMethods.ColorRef(r, g, b);
					break;
				case 12:
					csbe.red = new NativeMethods.ColorRef(r, g, b);
					break;
				case 13:
					csbe.magenta = new NativeMethods.ColorRef(r, g, b);
					break;
				case 14:
					csbe.yellow = new NativeMethods.ColorRef(r, g, b);
					break;
				case 15:
					csbe.white = new NativeMethods.ColorRef(r, g, b);
					break;
			}

			++csbe.srWindow.Bottom;
			++csbe.srWindow.Right;

			brc = NativeMethods.SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
			if (!brc) {
				return Marshal.GetLastWin32Error();
			}
			return 0;
		}
	}
}