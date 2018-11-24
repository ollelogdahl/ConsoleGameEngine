namespace ConsoleGameEngine {
	using System;
	using System.Runtime.InteropServices;

	static class ConsoleFontSize {
		public static int SetFontSize(IntPtr h, short sizeX, short sizeY) {
			if (h == new IntPtr(-1)) {
				return Marshal.GetLastWin32Error();
			}

			ConsoleHelper.CONSOLE_FONT_INFO_EX cfi = new ConsoleHelper.CONSOLE_FONT_INFO_EX();
			cfi.cbSize = (uint)Marshal.SizeOf(cfi);
			cfi.nFont = 0;
			cfi.dwFontSize.X = sizeX;
			cfi.dwFontSize.Y = sizeY;
			cfi.FaceName = "Consolas";

			ConsoleHelper.SetCurrentConsoleFontEx(h, true, ref cfi);
			return 0;
		}
	}
}
