namespace ConsoleGameEngine {
	using System;
	using System.Runtime.InteropServices;

	class ConsolePalette {

		public static int SetColor(int consoleColor, Color targetColor) {
			return SetColor(consoleColor, targetColor.R, targetColor.G, targetColor.B);
		}

		private static int SetColor(int color, uint r, uint g, uint b) {
			ConsoleHelper.CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new ConsoleHelper.CONSOLE_SCREEN_BUFFER_INFO_EX();
			csbe.cbSize = Marshal.SizeOf(csbe);
			IntPtr hConsoleOutput = ConsoleHelper.GetStdHandle(-11);
			if (hConsoleOutput == new IntPtr(-1)) {
				return Marshal.GetLastWin32Error();
			}
			bool brc = ConsoleHelper.GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
			if (!brc) {
				return Marshal.GetLastWin32Error();
			}

			switch (color) {
				case 0:
					csbe.black = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 1:
					csbe.darkBlue = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 2:
					csbe.darkGreen = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 3:
					csbe.darkCyan = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 4:
					csbe.darkRed = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 5:
					csbe.darkMagenta = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 6:
					csbe.darkYellow = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 7:
					csbe.gray = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 8:
					csbe.darkGray = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 9:
					csbe.blue = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 10:
					csbe.green = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 11:
					csbe.cyan = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 12:
					csbe.red = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 13:
					csbe.magenta = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 14:
					csbe.yellow = new ConsoleHelper.ColorRef(r, g, b);
					break;
				case 15:
					csbe.white = new ConsoleHelper.ColorRef(r, g, b);
					break;
			}

			++csbe.srWindow.Bottom;
			++csbe.srWindow.Right;

			brc = ConsoleHelper.SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
			if (!brc) {
				return Marshal.GetLastWin32Error();
			}
			return 0;
		}
	}
}