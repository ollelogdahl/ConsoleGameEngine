using System;

namespace ConsoleGameEngine {
	/// <summary> Enum for basic Unicodes. </summary>
	public enum ConsoleCharacter {
		Null = 0x0000,

		Full = 0x2588,
		Dark = 0x2593,
		Medium = 0x2592,
		Light = 0x2591,

		// box drawing syboler
		// ┌───────┐
		// │       │
		// │       │
		// └───────┘
		BoxDrawingL_H = 0x2500,
		BoxDrawingL_V = 0x2502,
		BoxDrawingL_DR = 0x250C,
		BoxDrawingL_DL = 0x2510,
		BoxDrawingL_UL = 0x2518,
		BoxDrawingL_UR = 0x2514,
	}

	/// <summary> Enum for Different Gameloop modes. </summary>
	public enum FramerateMode {
		/// <summary>Run at max speed, but no higher than TargetFramerate.</summary>
		MaxFps,
		/// <summary>Run at max speed.</summary>
		Unlimited
	}

	/// <summary> Represents prebuilt palettes. </summary>
	public static class Palettes {
		/// <summary> Pico8 palette. </summary>
		public static Color[] Pico8 { get; set; } = new Color[16] {
			new Color(0,    0,     0),
			new Color(29,   43,    83),
			new Color(126,  37,    83),
			new Color(0,    135,   81),
			new Color(171,  82,    54),
			new Color(95,   87,    79),
			new Color(194,  195,   199),
			new Color(255,  241,   232),
			new Color(255,  0,     77),
			new Color(255,  163,   0),
			new Color(255,  236,   39),
			new Color(0,    228,   54),
			new Color(41,   173,   255),
			new Color(131,  118,   156),
			new Color(255,  119,   168),
			new Color(255,  204,   170),
		};

		/// <summary> default windows console palette. </summary>
		public static Color[] Default { get; set; } = new Color[16] {
			new Color(12,   12,     12),			// Black
			new Color(0,    55,     218),			// DarkBlue
			new Color(19,   161,    14),			// DarkGreen
			new Color(58,   150,    221),			// DarkCyan
			new Color(197,  15,     31),			// DarkRed
			new Color(136,  23,     152),			// DarkMagenta
			new Color(193,  156,    0),				// DarkYellow
			new Color(204,  204,    204),			// Gray
			new Color(118,  118,    118),			// DarkGray
			new Color(59,   120,    255),			// Blue
			new Color(22,   192,    12),			// Green
			new Color(97,   214,    214),			// Cyan
			new Color(231,  72,     86),			// Red
			new Color(180,  0,      158),			// Magenta
			new Color(249,  241,    165),			// Yellow
			new Color(242,  242,    242),			// White
		};


		/// <summary> Color constants for ease of use ex: Palettes.BlUE</summary>
		public static readonly int NULL = -1;
		public static readonly int BLACK = 0;
		public static readonly int DARK_BLUE = 1;
		public static readonly int DARK_GREEN = 2;
		public static readonly int DARK_CYAN = 3;
		public static readonly int DARK_RED = 4;
		public static readonly int DARK_MAGENTA = 5;
		public static readonly int DARK_YELLOW = 6;
		public static readonly int GRAY = 7;
		public static readonly int DARK_GRAY = 8;
		public static readonly int BLUE = 9;
		public static readonly int GREEN = 10;
		public static readonly int CYAN = 11;
		public static readonly int RED = 12;
		public static readonly int MAGENTA = 13;
		public static readonly int YELLOW = 14;
		public static readonly int WHITE = 15;

		private static String[] COLOR_NAME = new String[]
		{
			"Black","Dark Blue","Dark Green","Dark Cyan","Dark Red","Dark Magenta","Dark Yellow","Gray","Dark Gray","Blue","Green","Cyan","Red","Magenta","Yellow","White"
		};

		/// <summary>toString function, which returns the string name of the color</summary>
		/// <param name="colorPosition">position in array</param>
		/// <returns>the name of the color in the palette array</returns>
		public static String ColorName(int colorPosition)
		{
			if (colorPosition < 0 || colorPosition > COLOR_NAME.Length)
				return "null";
			return COLOR_NAME[colorPosition];
		}


	}
}
