namespace ConsoleGameEngine {
	/* ----------------------------------------------------------------------
	 * ConsoleGameEngine.cs
	 * ----------------------------------------------------------------------
	 * A graphics library wrapping windows api for the windows command prompt
	 *
	 * Olle Logdahl, November 01 2018
	 * Licence: http://unlicense.org/
	 * ----------------------------------------------------------------------
	 * 
	 * This is the main graphics class that contains methods to set pixels,
	 * draw primitives clear and update screenbuffer, change color palette
	 * etc.
	 */

	using System;

	/// <include file='docs.xml' path='docs/members[@name="engine"]/ConsoleEngine/*'/>
	public class ConsoleEngine {
		public Color[] Palette { get; private set; }
		public Point FontSize { get; private set; }
		public Point WindowSize { get; private set; }

		private char[,] CharBuffer { get; set; }
		private int[,] ColorBuffer { get; set; }
		private int Background { get; set; }

		private ConsoleBuffer ConsoleBuffer { get; set; }
		private bool IsBorderless { get; set; }

		private readonly IntPtr stdInputHandle = ConsoleHelper.GetStdHandle(-10);
		private readonly IntPtr stdOutputHandle = ConsoleHelper.GetStdHandle(-11);
		private readonly IntPtr stdErrorHandle = ConsoleHelper.GetStdHandle(-12);
		private readonly IntPtr consoleHandle = ConsoleHelper.GetConsoleWindow();

		public ConsoleEngine(int width = 32, int height = 32, int fontW = 8, int fontH = 8) {
			if (width < 1 || height < 1) throw new ArgumentOutOfRangeException();
			if (fontW < 2 || fontH < 2) throw new ArgumentOutOfRangeException();

			Console.Title = "";
			Console.CursorVisible = false;

			ConsoleBuffer = new ConsoleBuffer(width, height);

			WindowSize = new Point(width, height);
			FontSize = new Point(fontW, fontH);

			// Stänger av alla standard ConsoleInput metoder (Quick-edit etc)
			ConsoleHelper.SetConsoleMode(stdInputHandle, 0x0080);
			// Sätter fontstorlek och tvingar Consolas
			ConsoleFontSize.SetFontSize(stdOutputHandle, (short)fontW, (short)fontH);

			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, height);

			CharBuffer = new char[width, height];
			ColorBuffer = new int[width, height];

			SetBackground(0);
			SetPalette(Palettes.Default);

			// sätter igen, idk men det behövs
			Console.SetWindowSize(width, height);
		}

		// Rita
		private void SetPixel(Point selectedPoint, char character, int color = 0) {
			if (selectedPoint.X >= CharBuffer.GetLength(0) || selectedPoint.Y >= CharBuffer.GetLength(1)
				|| selectedPoint.X < 0 || selectedPoint.Y < 0) return;

			CharBuffer[selectedPoint.X, selectedPoint.Y] = character;
			ColorBuffer[selectedPoint.X, selectedPoint.Y] = color;
			
		}
		public void SetPalette(Color[] colors) {
			if (colors.Length > 16) throw new OverflowException();
			Palette = colors ?? throw new ArgumentNullException();

			for (int i = 0; i < colors.Length; i++) {
				ConsolePalette.SetColor(i, colors[i]);
			}
		}
		public void SetBackground(int color = 0) {
			if (color > 16 || color < 0) throw new IndexOutOfRangeException();
			Background = color;
		}

		/// <include file='docs.xml' path='docs/members[@name="engine"]/ClearBuffer/*'/>
		public void ClearBuffer() {
			Array.Clear(CharBuffer, 0, CharBuffer.Length);
			Array.Clear(ColorBuffer, 0, ColorBuffer.Length);
		}
		/// <include file='docs.xml' path='docs/members[@name="engine"]/DisplayBuffer/*'/>
		public void DisplayBuffer() {
			ConsoleBuffer.SetBuffer(CharBuffer, ColorBuffer, Background);
			ConsoleBuffer.Blit();
		}

		#region Primitives

		public void SetPixel(Point v, ConsoleCharacter c, int color = 0) {
			SetPixel(v, (char)c, color);
		}

		public void ColorMatrix(Point c, int[,] colors) {
			for (int y = 0; y < colors.GetLength(1); y++) {
				for (int x = 0; x < colors.GetLength(0); x++) {
					ColorBuffer[c.X + x, c.Y + y] = colors[x, y];
				}
			}
		}

		public void Window(Point pos, Point end, int color) {
			for (int i = 1; i < end.X - pos.X; i++) {
				SetPixel(new Point(pos.X + i, pos.Y), ConsoleCharacter.BoxDrawingL_H, color);
				SetPixel(new Point(pos.X + i, end.Y), ConsoleCharacter.BoxDrawingL_H, color);
			}

			for (int i = 1; i < end.Y - pos.Y; i++) {
				SetPixel(new Point(pos.X, pos.Y + i), ConsoleCharacter.BoxDrawingL_V, color);
				SetPixel(new Point(end.X, pos.Y + i), ConsoleCharacter.BoxDrawingL_V, color);
			}

			SetPixel(new Point(pos.X, pos.Y), ConsoleCharacter.BoxDrawingL_DR, color);
			SetPixel(new Point(end.X, pos.Y), ConsoleCharacter.BoxDrawingL_DL, color);
			SetPixel(new Point(pos.X, end.Y), ConsoleCharacter.BoxDrawingL_UR, color);
			SetPixel(new Point(end.X, end.Y), ConsoleCharacter.BoxDrawingL_UL, color);
		}

		public void WriteText(Point pos, string text, int color) {
			for (int i = 0; i < text.Length; i++) {
				SetPixel(new Point(pos.X + i, pos.Y), text[i], color);
			}
		}

		public void Arc(Point pos, int radius, int col, int arc = 360) {
			for (int a = 0; a < arc; a++) {
				int x = (int)(radius * Math.Cos((float)a / 57.29577f));
				int y = (int)(radius * Math.Sin((float)a / 57.29577f));

				Point v = new Point(pos.X + x, pos.Y + y);
				SetPixel(v, ConsoleCharacter.Full, col);
			}
		}
		public void SemiCircle(Point pos, int radius, int col, int start = 0, int arc = 360, ConsoleCharacter chr = ConsoleCharacter.Full) {
			for (int a = start; a > -arc + start; a--) {
				for (int r = 0; r < radius + 1; r++) {
					int x = (int)(r * Math.Cos((float)a / 57.29577f));
					int y = (int)(r * Math.Sin((float)a / 57.29577f));

					Point v = new Point(pos.X + x, pos.Y + y);
					SetPixel(v, chr, col);
				}
			}
		}

		// Bresenhams Line Algorithm
		// https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		public void Line(Point start, Point end, int color, ConsoleCharacter c = ConsoleCharacter.Full) {
			Point delta = end - start;
			Point da = Point.Zero, db = Point.Zero;
			if (delta.X < 0) da.X = -1; else if (delta.X > 0) da.X = 1;
			if (delta.Y < 0) da.Y = -1; else if (delta.Y > 0) da.Y = 1;
			if (delta.X < 0) db.X = -1; else if (delta.X > 0) db.X = 1;
			int longest = Math.Abs(delta.X);
			int shortest = Math.Abs(delta.Y);

			if (!(longest > shortest)) {
				longest = Math.Abs(delta.Y);
				shortest = Math.Abs(delta.X);
				if (delta.Y < 0) db.Y = -1; else if (delta.Y > 0) db.Y = 1;
				db.X = 0;
			}

			int numerator = longest >> 1;
			Point p = new Point(start.X, start.Y);
			for (int i = 0; i <= longest; i++) {
				SetPixel(p, c, color);
				numerator += shortest;
				if (!(numerator < longest)) {
					numerator -= longest;
					p += da;
				} else {
					p += db;
				}
			}
		}

		public void Rectangle(Point pos, Point end, int col = 0, ConsoleCharacter chr = ConsoleCharacter.Full) {
			for (int i = 0; i < end.X - pos.X; i++) {
				SetPixel(new Point(pos.X + i, pos.Y), chr, col);
				SetPixel(new Point(pos.X + i, end.Y), chr, col);
			}

			for (int i = 0; i < end.Y - pos.Y + 1; i++) {
				SetPixel(new Point(pos.X, pos.Y + i), chr, col);
				SetPixel(new Point(end.X, pos.Y + i), chr, col);
			}
		}
		public void Fill(Point a, Point b, ConsoleCharacter c, int color) {
			for (int y = a.Y; y < b.Y; y++) {
				for (int x = a.X; x < b.X; x++) {
					SetPixel(new Point(x, y), (char)c, color);
				}
			}
		}

		public void Triangle(Point a, Point b, Point c, int col, ConsoleCharacter character) {
			Line(a, b, col, character);
			Line(b, c, col, character);
			Line(c, a, col, character);
		}

		public void FillTriangle(Point a, Point b, Point c, int col, ConsoleCharacter character) {
			Point min = new Point(Math.Min(Math.Min(a.X, b.X), c.X), Math.Min(Math.Min(a.Y, b.Y), c.Y));
			Point max = new Point(Math.Max(Math.Max(a.X, b.X), c.X), Math.Max(Math.Max(a.Y, b.Y), c.Y));

			Point p = new Point();
			for (p.Y = min.Y; p.Y < max.Y; p.Y++) {
				for (p.X = min.X; p.X < max.X; p.X++) {
					int w0 = Orient(b, c, p);
					int w1 = Orient(c, a, p);
					int w2 = Orient(a, b, p);

					if (w0 >= 0 && w1 >= 0 && w2 >= 0) SetPixel(p, character, col);
				}
			}
		}
		int Orient(Point a, Point b, Point c) {
			return ((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X));
		}

		#endregion

		// ändrar om konsolen ska vara borderless, normal, frame etc.
		public void Borderless(bool b) {
			IsBorderless = b;

			int GWL_STYLE = -16;				// hex konstant för stil-förändring
			int WS_DEFAULT =	0x00C00000;		// vanlig
			int WS_BORDERLESS = 0x00080000;     // helt borderless

			ConsoleHelper.Rect rect = new ConsoleHelper.Rect();
			ConsoleHelper.Rect desktopRect = new ConsoleHelper.Rect();

			ConsoleHelper.GetWindowRect(consoleHandle, ref rect);
			IntPtr desktopHandle = ConsoleHelper.GetDesktopWindow();
			ConsoleHelper.MapWindowPoints(desktopHandle, consoleHandle, ref rect, 2);
			ConsoleHelper.GetWindowRect(desktopHandle, ref desktopRect);

			Point wPos = new Point(
				(desktopRect.Right  / 2) - ((WindowSize.X * FontSize.X) / 2),
				(desktopRect.Bottom / 2) - ((WindowSize.Y * FontSize.Y) / 2));

			if(b == true) {
				ConsoleHelper.SetWindowLong(consoleHandle, GWL_STYLE, WS_BORDERLESS);
				ConsoleHelper.SetWindowPos(consoleHandle, -2, wPos.X, wPos.Y, rect.Right - 8, rect.Bottom - 8, 0x0040);
			} else {
				ConsoleHelper.SetWindowLong(consoleHandle, GWL_STYLE, WS_DEFAULT);
				ConsoleHelper.SetWindowPos(consoleHandle, -2, wPos.X, wPos.Y, rect.Right, rect.Bottom, 0x0040);
			}

			ConsoleHelper.DrawMenuBar(consoleHandle);
		}

		// Input
		public bool GetKey(ConsoleKey key) {
			short s = ConsoleHelper.GetAsyncKeyState((Int32)key);
			return (s & 0x8000) > 0;
		}
		public bool GetKeyDown(ConsoleKey key) {
			int s = Convert.ToInt32(ConsoleHelper.GetAsyncKeyState((Int32)key));
			return (s == -32767);
		}
		public bool GetMouseLeft() {
			short s = ConsoleHelper.GetAsyncKeyState(0x01);
			return (s & 0x8000) > 0;
		}

		public Point GetMousePos() {
			ConsoleHelper.Rect r = new ConsoleHelper.Rect();
			ConsoleHelper.GetWindowRect(consoleHandle, ref r);

			if (ConsoleHelper.GetCursorPos(out ConsoleHelper.POINT p)) {
				Point point = new Point();
				if (!IsBorderless) {
					p.Y -= 29;
					point = new Point(
						(int)Math.Floor(((p.X - r.Left) / (float)FontSize.X) - 0.5f),
						(int)Math.Floor(((p.Y - r.Top) / (float)FontSize.Y))
					);
				} else {
					point = new Point(
						(int)Math.Floor(((p.X - r.Left) / (float)FontSize.X)),
						(int)Math.Floor(((p.Y - r.Top) / (float)FontSize.Y))
					);
				}
				return new Point(Utility.Clamp(point.X, 0, WindowSize.X - 1), Utility.Clamp(point.Y, 0, WindowSize.Y - 1));
			}

			throw new Exception();
		}
	}
}
