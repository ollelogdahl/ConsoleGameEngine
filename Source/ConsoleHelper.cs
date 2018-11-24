namespace ConsoleGameEngine {
	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using Microsoft.Win32.SafeHandles;

	// En helper klass för att wrappa User32 och Kernel32 dll
	// Används för
	class ConsoleHelper {

		#region Signatures

		[DllImport("user32.dll", SetLastError = true)]
		public static extern short GetAsyncKeyState(Int32 vKey);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetCursorPos(out POINT vKey);


		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool DrawMenuBar(IntPtr hWnd);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref Rect rect, [MarshalAs(UnmanagedType.U4)] int cPoints);



		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern SafeFileHandle CreateFile(
			string fileName,
			[MarshalAs(UnmanagedType.U4)] uint fileAccess,
			[MarshalAs(UnmanagedType.U4)] uint fileShare,
			IntPtr securityAttributes,
			[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
			[MarshalAs(UnmanagedType.U4)] int flags,
		IntPtr template);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteConsoleOutputW(
			SafeFileHandle hConsoleOutput,
			CharInfo[] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
		ref SmallRect lpWriteRegion);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetConsoleScreenBufferInfoEx( IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe );

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleScreenBufferInfoEx( IntPtr ConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe );

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern Int32 SetCurrentConsoleFontEx(
		IntPtr ConsoleOutput,
		bool MaximumWindow,
		ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

		#endregion

		#region Structs

		// Basic
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int X;
			public int Y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Coord {
			public short X;
			public short Y;

			public Coord(short X, short Y) {
				this.X = X;
				this.Y = Y;
			}
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct Rect {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct SmallRect {
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}

		// Tecken, används av buffern
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct CharInfo {
			[FieldOffset(0)] public char UnicodeChar;
			[FieldOffset(0)] public byte AsciiChar;
			[FieldOffset(2)] public short Attributes;
		}

		// Används for att ändra ColorRef, custom palette :)
		[StructLayout(LayoutKind.Sequential)]
		public struct ColorRef {
			internal uint ColorDWORD;

			internal ColorRef(Color color) {
				ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
			}

			internal ColorRef(uint r, uint g, uint b) {
				ColorDWORD = r + (g << 8) + (b << 16);
			}

			internal Color GetColor() {
				return new Color((int)(0x000000FFU & ColorDWORD),
				   (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
			}

			internal void SetColor(Color color) {
				ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CONSOLE_SCREEN_BUFFER_INFO_EX {
			public int cbSize;
			public Coord dwSize;
			public Coord dwCursorPosition;
			public short wAttributes;
			public SmallRect srWindow;
			public Coord dwMaximumWindowSize;

			public ushort wPopupAttributes;
			public bool bFullscreenSupported;

			internal ColorRef black;
			internal ColorRef darkBlue;
			internal ColorRef darkGreen;
			internal ColorRef darkCyan;
			internal ColorRef darkRed;
			internal ColorRef darkMagenta;
			internal ColorRef darkYellow;
			internal ColorRef gray;
			internal ColorRef darkGray;
			internal ColorRef blue;
			internal ColorRef green;
			internal ColorRef cyan;
			internal ColorRef red;
			internal ColorRef magenta;
			internal ColorRef yellow;
			internal ColorRef white;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CONSOLE_FONT_INFO_EX {
			public uint cbSize;
			public uint nFont;
			public Coord dwFontSize;
			public int FontFamily;
			public int FontWeight;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
			public string FaceName;
		}

		#endregion
	}
}
