namespace ConsoleGameEngine {
	using System;
	using System.Runtime.InteropServices;

	static class ConsoleFontSize {
		internal static int SetFontSize(IntPtr h, short sizeX, short sizeY) {
			if (h == new IntPtr(-1)) {
				return Marshal.GetLastWin32Error();
			}

			NativeMethods.CONSOLE_FONT_INFO_EX cfi = new NativeMethods.CONSOLE_FONT_INFO_EX();
			cfi.cbSize = (uint)Marshal.SizeOf(cfi);
			cfi.nFont = 0;
			cfi.dwFontSize.X = sizeX;
			cfi.dwFontSize.Y = sizeY;
			cfi.FaceName = "Consolas";

			NativeMethods.SetCurrentConsoleFontEx(h, true, ref cfi);
			return 0;
		}
	}
}
