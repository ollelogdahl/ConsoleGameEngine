namespace ConsoleGameEngine {
	using System;
	using System.Runtime.InteropServices;

	using System.Drawing;

	class ConsoleFont {



		internal static int SetFont(IntPtr h, short sizeX, short sizeY) {
			if (h == new IntPtr(-1)) {
				return Marshal.GetLastWin32Error();
			}

			NativeMethods.CONSOLE_FONT_INFO_EX cfi = new NativeMethods.CONSOLE_FONT_INFO_EX();
			cfi.cbSize = (uint)Marshal.SizeOf(cfi);
			cfi.nFont = 0;

			cfi.dwFontSize.X = sizeX;
			cfi.dwFontSize.Y = sizeY;

			// forcerar font till Terminal (Raster)
			cfi.FaceName = "Terminal";

			NativeMethods.SetCurrentConsoleFontEx(h, true, ref cfi);
			return 0;
		}
	}
}
