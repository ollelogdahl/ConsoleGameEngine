namespace ConsoleGameEngine {

	using System;
	using System.Text;

	/// <summary>
	/// Class for Drawing to a console window.
	/// </summary>
	public class ConsoleEngine {

		// English please!
		// pekare för ConsoleHelper-anrop
		private readonly IntPtr stdInputHandle = NativeMethods.GetStdHandle(-10);
		private readonly IntPtr stdOutputHandle = NativeMethods.GetStdHandle(-11);
		private readonly IntPtr stdErrorHandle = NativeMethods.GetStdHandle(-12);
		private readonly IntPtr consoleHandle = NativeMethods.GetConsoleWindow();

		/// <summary> The active color palette. </summary> <see cref="Color"/>
		public Color[] Palette { get; private set; }

		/// <summary> The current size of the font. </summary> <see cref="Point"/>
		public Point FontSize { get; private set; }

		/// <summary> The dimensions of the window in characters. </summary> <see cref="Point"/>
		public Point WindowSize { get; private set; }

		/*private char[,] CharBuffer { get; set; }
		private int[,] ColorBuffer { get; set; }
		private int[,] BackgroundBuffer { get; set; }*/
		private Glyph[,] GlyphBuffer { get; set; }  
		private int Background { get; set; }
		private ConsoleBuffer ConsoleBuffer { get; set; }
		private bool IsBorderless { get; set; }

		/// <summary> Creates a new ConsoleEngine. </summary>
		/// <param name="width">Target window width.</param>
		/// <param name="height">Target window height.</param>
		/// <param name="fontW">Target font width.</param>
		/// <param name="fontH">Target font height.</param>
		public ConsoleEngine(int width, int height, int fontW, int fontH) {
			if (width < 1 || height < 1) throw new ArgumentOutOfRangeException();
			if (fontW < 1 || fontH < 1) throw new ArgumentOutOfRangeException();

			Console.Title = "Untitled console application";
			Console.CursorVisible = false;

			//sets console location to 0,0 to try and avoid the error where the console is to big
			Console.SetWindowPosition(0, 0);

			// sätter fönstret och bufferns storlek
			// buffern måste sättas efter fönsret, eftersom den aldrig får vara mindre än skärmen
			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, height);

			ConsoleBuffer = new ConsoleBuffer(width, height);

			WindowSize = new Point(width, height);
			FontSize = new Point(fontW, fontH);

			/*CharBuffer = new char[width, height];
			ColorBuffer = new int[width, height];
			BackgroundBuffer = new int[width, height];*/

			GlyphBuffer = new Glyph[width, height];
			for(int y = 0; y < GlyphBuffer.GetLength(1); y++)
				for(int x = 0; x < GlyphBuffer.GetLength(0); x++)
					GlyphBuffer[x, y] = new Glyph();

			

			SetBackground(0);
			SetPalette(Palettes.Default);

			// Stänger av alla standard ConsoleInput metoder (Quick-edit etc)
			NativeMethods.SetConsoleMode(stdInputHandle, 0x0080);

			// Sätter fontstorlek och tvingar Raster (Terminal) / Consolas
			// Detta måste göras efter SetBufferSize, annars ger den en IOException
			ConsoleFont.SetFont(stdOutputHandle, (short)fontW, (short)fontH);
		}

		// Rita
		public void SetPixel(Point selectedPoint, int color, char character)
		{
			SetPixel(selectedPoint,color,Background,character);
		}

		//new Draw method, which supports background
		public void SetPixel(Point selectedPoint, int fgColor, int bgColor, char character) {
			if (selectedPoint.X >= GlyphBuffer.GetLength(0) || selectedPoint.Y >= GlyphBuffer.GetLength(1)
				|| selectedPoint.X < 0 || selectedPoint.Y < 0) return;

			/*CharBuffer[selectedPoint.X, selectedPoint.Y] = character;
			ColorBuffer[selectedPoint.X, selectedPoint.Y] = fgColor;
			BackgroundBuffer[selectedPoint.X, selectedPoint.Y] = bgColor;*/
			GlyphBuffer[selectedPoint.X, selectedPoint.Y].set(character, fgColor, bgColor);
		}

		/// <summary>
		/// returns glyph at point given
		/// </summary>
		/// <param name="selectedPoint"></param>
		/// <returns></returns>
		public Glyph PixelAt(Point selectedPoint)
		{
			if(selectedPoint.X > 0 && selectedPoint.X < GlyphBuffer.GetLength(0) && selectedPoint.Y > 0 && selectedPoint.Y < GlyphBuffer.GetLength(1))
				return GlyphBuffer[selectedPoint.X, selectedPoint.Y];
			else return null;
		}


		/// <summary> Sets the console's color palette </summary>
		/// <param name="colors"></param>
		/// <exception cref="ArgumentException"/> <exception cref="ArgumentNullException"/>
		public void SetPalette(Color[] colors) {
			if (colors.Length > 16) throw new ArgumentException("Windows command prompt only support 16 colors.");
			Palette = colors ?? throw new ArgumentNullException();

			for (int i = 0; i < colors.Length; i++) {
				ConsolePalette.SetColor(i, colors[i]);
			}
		}

		/// <summary> Sets the console's background color to one in the active palette. </summary>
		/// <param name="color">Index of background color in palette.</param>
		public void SetBackground(int color = 0) {
			if (color > 16 || color < 0) throw new IndexOutOfRangeException();
			Background = color;
		}

		/// <summary>Gets Background</summary>
		/// <returns>Returns the background</returns>
		public int GetBackground()
		{
			return Background;
		}

		/// <summary> Clears the screenbuffer. </summary>
		public void ClearBuffer() {
			/*Array.Clear(CharBuffer, 0, CharBuffer.Length);
			Array.Clear(ColorBuffer, 0, ColorBuffer.Length);
			Array.Clear(BackgroundBuffer, 0, BackgroundBuffer.Length);*/
			Array.Clear(GlyphBuffer, 0, GlyphBuffer.Length);
		}

		/// <summary> Blits the screenbuffer to the Console window. </summary>
		public void DisplayBuffer() {
			ConsoleBuffer.SetBuffer(GlyphBuffer, Background);
			ConsoleBuffer.Blit();
		}

		/// <summary> Sets the window to borderless mode. </summary>
		public void Borderless() {
			IsBorderless = true;

			int GWL_STYLE = -16;                // hex konstant för stil-förändring
			int WS_BORDERLESS = 0x00080000;     // helt borderless

			NativeMethods.Rect rect = new NativeMethods.Rect();
			NativeMethods.Rect desktopRect = new NativeMethods.Rect();

			NativeMethods.GetWindowRect(consoleHandle, ref rect);
			IntPtr desktopHandle = NativeMethods.GetDesktopWindow();
			NativeMethods.MapWindowPoints(desktopHandle, consoleHandle, ref rect, 2);
			NativeMethods.GetWindowRect(desktopHandle, ref desktopRect);

			Point wPos = new Point(
				(desktopRect.Right / 2) - ((WindowSize.X * FontSize.X) / 2),
				(desktopRect.Bottom / 2) - ((WindowSize.Y * FontSize.Y) / 2));

			NativeMethods.SetWindowLong(consoleHandle, GWL_STYLE, WS_BORDERLESS);
			NativeMethods.SetWindowPos(consoleHandle, -2, wPos.X, wPos.Y, rect.Right - 8, rect.Bottom - 8, 0x0040);

			NativeMethods.DrawMenuBar(consoleHandle);
		}

		#region Primitives

		/// <summary> Draws a single pixel to the screenbuffer. calls new method with Background as the bgColor </summary>
		/// <param name="v">The Point that should be drawn to.</param>
		/// <param name="color">The color index.</param>
		/// <param name="c">The character that should be drawn with.</param>
		public void SetPixel(Point v, int color, ConsoleCharacter c = ConsoleCharacter.Full) {
			SetPixel(v, color, Background, (char)c);
		}

		/// <summary> Overloaded Method Draws a single pixel to the screenbuffer with custom bgColor. </summary>
		/// <param name="v">The Point that should be drawn to.</param>
		/// <param name="fgColor">The foreground color index.</param>
		/// <param name="bgColor">The background color index.</param>
		/// <param name="c">The character that should be drawn with.</param>
		public void SetPixel(Point v, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			SetPixel(v, fgColor, bgColor, (char)c);
		}

		/// <summary> Draws a frame using boxdrawing symbols, calls new method with Background as the bgColor. </summary>
		/// <param name="pos">Top Left corner of box.</param>
		/// <param name="end">Bottom Right corner of box.</param>
		/// <param name="color">The specified color index.</param>
		public void Frame(Point pos, Point end, int color) {
			Frame(pos,end,color,Background);
		}

		/// <summary> Draws a frame using boxdrawing symbols. </summary>
		/// <param name="pos">Top Left corner of box.</param>
		/// <param name="end">Bottom Right corner of box.</param>
		/// <param name="fgColor">The specified color index.</param>
		/// <param name="bgColor">The specified background color index.</param>
		public void Frame(Point pos, Point end, int fgColor, int bgColor)
		{
			for(int i = 1; i < end.X - pos.X; i++) {
				SetPixel(new Point(pos.X + i, pos.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_H);
				SetPixel(new Point(pos.X + i, end.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_H);
			}

			for(int i = 1; i < end.Y - pos.Y; i++) {
				SetPixel(new Point(pos.X, pos.Y + i), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_V);
				SetPixel(new Point(end.X, pos.Y + i), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_V);
			}

			SetPixel(new Point(pos.X, pos.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_DR);
			SetPixel(new Point(end.X, pos.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_DL);
			SetPixel(new Point(pos.X, end.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_UR);
			SetPixel(new Point(end.X, end.Y), fgColor, bgColor, ConsoleCharacter.BoxDrawingL_UL);
		}

		/// <summary> Writes plain text to the buffer, calls new method with Background as the bgColor. </summary>
		/// <param name="pos">The position to write to.</param>
		/// <param name="text">String to write.</param>
		/// <param name="color">Specified color index to write with.</param>
		public void WriteText(Point pos, string text, int color) {
			WriteText(pos,text,color,Background);
		}

		/// <summary> Writes plain text to the buffer. </summary>
		/// <param name="pos">The position to write to.</param>
		/// <param name="text">String to write.</param>
		/// <param name="fgColor">Specified color index to write with.</param>
		/// <param name="bgColor">Specified background color index to write with.</param>
		public void WriteText(Point pos, string text, int fgColor, int bgColor)
		{
			for(int i = 0; i < text.Length; i++) {
				SetPixel(new Point(pos.X + i, pos.Y), fgColor,bgColor, text[i]);
			}
		}

		/// <summary>  Writes text to the buffer in a FIGlet font, calls new method with Background as the bgColor. </summary>
		/// <param name="pos">The Top left corner of the text.</param>
		/// <param name="text">String to write.</param>
		/// <param name="font">FIGLET font to write with.</param>
		/// <param name="color">Specified color index to write with.</param>
		/// <see cref="FigletFont"/>
		public void WriteFiglet(Point pos, string text, FigletFont font, int color) {
			WriteFiglet(pos, text, font, color, Background);
		}

		/// <summary>  Writes text to the buffer in a FIGlet font. </summary>
		/// <param name="pos">The Top left corner of the text.</param>
		/// <param name="text">String to write.</param>
		/// <param name="font">FIGLET font to write with.</param>
		/// <param name="fgColor">Specified color index to write with.</param>
		/// <param name="bgColor">Specified background color index to write with.</param>
		/// <see cref="FigletFont"/>
		public void WriteFiglet(Point pos, string text, FigletFont font, int fgColor, int bgColor)
		{
			if(text == null) throw new ArgumentNullException(nameof(text));
			if(Encoding.UTF8.GetByteCount(text) != text.Length) throw new ArgumentException("String contains non-ascii characters");

			int sWidth = FigletFont.GetStringWidth(font, text);		// unused ?

			for(int line = 1; line <= font.Height; line++)
			{
				int runningWidthTotal = 0;

				foreach (var character in text)
				{
					string fragment = FigletFont.GetCharacter(font, character, line);
					for(int f = 0; f < fragment.Length; f++) {
						if(fragment[f] != ' ') {
							SetPixel(new Point(pos.X + runningWidthTotal + f, pos.Y + line - 1), fgColor,bgColor, fragment[f]);
						}
					}
					runningWidthTotal += fragment.Length;
				}
			}
		}

		/// <summary> Draws an Arc, calls new method with Background as the bgColor. </summary>
		/// <param name="pos">Center of Arc.</param>
		/// <param name="radius">Radius of Arc.</param>
		/// <param name="color">Specified color index.</param>
		/// <param name="arc">angle in degrees, 360 if not specified.</param>
		/// <param name="c">Character to use.</param>
		[Obsolete("For comparison purposes with Arc2. Delete later. Rename new to Arc.")]
		public void Arc(Point pos, int radius, int color, int arc = 360, ConsoleCharacter c = ConsoleCharacter.Full) {
			Arc(pos, radius, color, Background, arc, c);
		}

		/// <summary> Draws an Arc. </summary>
		/// <param name="pos">Center of Arc.</param>
		/// <param name="radius">Radius of Arc.</param>
		/// <param name="fgColor">Specified color index.</param>
		/// <param name="bgColor">Specified background color index.</param>
		/// <param name="arc">angle in degrees, 360 if not specified.</param>
		/// <param name="c">Character to use.</param>
		[Obsolete("For comparison purposes with Arc2. Delete later. Rename new to Arc.")]
		public void Arc(Point pos, int radius, int fgColor, int bgColor, int arc = 360, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			for(int a = 0; a < arc; a++) {
				int x = (int)(radius * Math.Cos((float)a / 57.29577f));
				int y = (int)(radius * Math.Sin((float)a / 57.29577f));

				Point v = new Point(pos.X + x, pos.Y + y);
				SetPixel(v, fgColor,bgColor, ConsoleCharacter.Full);
			}
		}

		/// <summary> Draws an Arc, calls new method with Background as the bgColor. </summary>
		/// <param name="pos">Center of Arc.</param>
		/// <param name="radius">Radius of Arc.</param>
		/// <param name="arcStart">Start angle in degrees. Drawing happens counterclockwise.</param>
		/// <param name="arcEnd">End angle in degrees. Drawing happens counterclockwise.</param>
		/// <param name="fgColor">Specified foreground color index.</param>
		/// <param name="c">Character to use.</param>
		/// <remarks>Will draws entire <c>Arc2</c> if <paramref name="arcStart"/> and <paramref name="arcEnd"/>
		/// normalized forms have same value.</remarks>
		// TODO: Arc and Sector have too many integers in a row - turn either color or start/end into a object
		public void Arc2(Point pos, int radius, int arcStart, int arcEnd, int fgColor, ConsoleCharacter c = ConsoleCharacter.Full) {
			Arc2(pos, radius, arcStart, arcEnd, fgColor, Background, c);
		}

		/// <summary> Draws an Arc. </summary>
		/// <param name="pos">Center of Arc.</param>
		/// <param name="radius">Radius of Arc.</param>
		/// <param name="arcStart">Start angle in degrees. Drawing happens counterclockwise.</param>
		/// <param name="arcEnd">End angle in degrees. Drawing happens counterclockwise.</param>
		/// <param name="fgColor">Specified foreground color index.</param>
		/// <param name="bgColor">Specified background color index.</param>
		/// <param name="c">Character to use.</param>
		/// <remarks>Will draws entire <c>Arc2</c> if <paramref name="arcStart"/> and <paramref name="arcEnd"/>
		/// normalized forms have same value.</remarks>
		public void Arc2(Point pos, int radius, int arcStart, int arcEnd, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			ArcSectorCommon(pos, radius, arcStart, arcEnd, fgColor,  bgColor, c);
		}

		/// <summary> Draws a filled Arc, calls new method with Background as the bgColor </summary>
		/// <param name="pos">Center of Arc.</param>
		/// <param name="radius">Radius of Arc.</param>
		/// <param name="start">Start angle in degrees.</param>
		/// <param name="arc">End angle in degrees.</param>
		/// <param name="color">Specified color index.</param>
		/// <param name="c">Character to use.</param>
		[Obsolete("For comparison purposes with Sector. Delete later. This is not even a semicircle.")]
		public void SemiCircle(Point pos, int radius, int start, int arc, int color, ConsoleCharacter c = ConsoleCharacter.Full) {
			SemiCircle(pos, radius, start, arc, color, Background, c);
		}

		/// <summary> Draws a filled Arc. </summary>
		/// <param name="pos">Center of Arc.</param>
		/// <param name="radius">Radius of Arc.</param>
		/// <param name="start">Start angle in degrees.</param>
		/// <param name="arc">End angle in degrees.</param>
		/// <param name="fgColor">Specified color index.</param>
		/// <param name="bgColor">Specified background color index.</param>
		/// <param name="c">Character to use.</param>
		[Obsolete("For comparison purposes with Sector. Delete later")]
		public void SemiCircle(Point pos, int radius, int start, int arc, int fgColor,int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			for(int a = start; a > -arc + start; a--) {
				for(int r = 0; r < radius + 1; r++) {
					int x = (int)(r * Math.Cos((float)a / 57.29577f));
					int y = (int)(r * Math.Sin((float)a / 57.29577f));

					Point v = new Point(pos.X + x, pos.Y + y);	// should be pos.Y - y
					SetPixel(v, fgColor, bgColor, c);
				}
			}
		}
		
		/// <summary> Draws a Circle. </summary>
		/// <param name="pos">Center of Sector.</param>
		/// <param name="radius">Radius of Sector.</param>
		/// <param name="fgColor">Specified foreground color index.</param>
		/// <param name="bgColor">Specified background color index.</param>
		/// <param name="c">Character to use.</param>
		public void Circle(Point pos, int radius, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full) {
			Sector(pos, radius, 1, 360, fgColor, bgColor, c);
		}

		/// <summary> Draws a Sector/filled Arc, calls new method with Background as the bgColor </summary>
		/// <param name="pos">Center of Sector.</param>
		/// <param name="radius">Radius of Sector.</param>
		/// <param name="sectorStart">Start angle in degrees. Drawing happens counterclockwise</param>
		/// <param name="sectorEnd">End angle in degrees. Drawing happens counterclockwise</param>
		/// <param name="fgColor">Specified foreground color index.</param>
		/// <param name="c">Character to use.</param>
		public void Sector(Point pos, int radius, int sectorStart, int sectorEnd, int fgColor, ConsoleCharacter c = ConsoleCharacter.Full) {
			Sector(pos, radius, sectorStart, sectorEnd, fgColor, Background, c);
		}

		/// <summary> Draws a Sector/filled Arc.</summary>
		/// <param name="pos">Center of Sector.</param>
		/// <param name="radius">Radius of Sector.</param>
		/// <param name="sectorStart">Start angle in degrees. Drawing happens counterclockwise</param>
		/// <param name="sectorEnd">End angle in degrees. Drawing happens counterclockwise</param>
		/// <param name="fgColor">Specified foreground color index.</param>
		/// <param name="bgColor">Specified background color index.</param>
		/// <param name="c">Character to use.</param>
		/// <param name="willDrawOnFullRotation">Will draw sector when both degrees have the same normalized value. Ex 0 and 360</param>
		/// <remarks>Will draw nothing if <paramref name="sectorStart"/> and <paramref name="sectorEnd"/>
		/// normalized forms have same value.</remarks>
		public void Sector(Point pos, int radius, int sectorStart, int sectorEnd, int fgColor, int bgColor, ConsoleCharacter c = ConsoleCharacter.Full, bool willDrawOnFullRotation = false)
		{
			int arcStartNorm = sectorStart;
			int arcEndNorm = sectorEnd;
			
			arcStartNorm %= 360;
			arcEndNorm %= 360;
			
			if (arcStartNorm < 1) { arcStartNorm += 360; }
			if (arcEndNorm < 1) { arcEndNorm += 360; }
			
			int loopLimit = arcEndNorm >= arcStartNorm ? arcEndNorm : 360 + arcEndNorm;

			if (arcStartNorm == arcEndNorm && willDrawOnFullRotation)
			{
				loopLimit += 360;
			}
			
			for (int r = 0; r < radius + 1; r++)
			{
				ArcSectorCommon(pos, r,  arcStartNorm, loopLimit, fgColor, bgColor, c);
			}
		}

		/// <summary> Draws an Arc. </summary>
		/// <param name="pos">Center of Arc.</param>
		/// <param name="radius">Radius of Arc.</param>
		/// <param name="arcStart">Start angle in degrees. Drawing happens counterclockwise.</param>
		/// <param name="arcEnd">End angle in degrees. Drawing happens counterclockwise.</param>
		/// <param name="fgColor">Specified foreground color index.</param>
		/// <param name="bgColor">Specified background color index.</param>
		/// <param name="c">Character to use.</param>
		private void ArcSectorCommon(Point pos, int radius, int arcStart, int arcEnd, int fgColor, int bgColor, ConsoleCharacter c)
		{
			int arcStartNorm = arcStart;
			int arcEndNorm = arcEnd;
			if (arcStartNorm == 0)	// will create extra pixel at middle-right if a = 0, a = 360 does not have this problem
			{
				arcStartNorm++;
				arcEndNorm++;
			}
			
			for(int a = arcStartNorm; a < arcEndNorm; a++) {
				int x = (int)(radius * Math.Cos(a / 57.29577f));
				int y = (int)(radius * Math.Sin(a / 57.29577f));

				Point v = new Point(pos.X + x, pos.Y - y);
				SetPixel(v, fgColor,bgColor, c);
			}
		}

		// Bresenhams Line Algorithm
		// https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		/// <summary> Draws a line from start to end. (Bresenhams Line), calls overloaded method with background as bgColor </summary>
		/// <param name="start">Point to draw line from.</param>
		/// <param name="end">Point to end line at.</param>
		/// <param name="color">Color to draw with.</param>
		/// <param name="c">Character to use.</param>
		public void Line(Point start, Point end, int color, ConsoleCharacter c = ConsoleCharacter.Full) {
			Line(start, end, color, Background, c);
		}

		// Bresenhams Line Algorithm
		// https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		/// <summary> Draws a line from start to end. (Bresenhams Line) </summary>
		/// <param name="start">Point to draw line from.</param>
		/// <param name="end">Point to end line at.</param>
		/// <param name="fgColor">Color to draw with.</param>
		/// <param name="bgColor">Color to draw the background with.</param>
		/// <param name="c">Character to use.</param>
		public void Line(Point start, Point end, int fgColor,int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			Point delta = end - start;
			Point da = Point.Zero, db = Point.Zero;
			if(delta.X < 0) da.X = -1; else if(delta.X > 0) da.X = 1;
			if(delta.Y < 0) da.Y = -1; else if(delta.Y > 0) da.Y = 1;
			if(delta.X < 0) db.X = -1; else if(delta.X > 0) db.X = 1;
			int longest = Math.Abs(delta.X);
			int shortest = Math.Abs(delta.Y);

			if(!(longest > shortest)) {
				longest = Math.Abs(delta.Y);
				shortest = Math.Abs(delta.X);
				if(delta.Y < 0) db.Y = -1; else if(delta.Y > 0) db.Y = 1;
				db.X = 0;
			}

			int numerator = longest >> 1;
			Point p = new Point(start.X, start.Y);
			for(int i = 0; i <= longest; i++) {
				SetPixel(p, fgColor,bgColor, c);
				numerator += shortest;
				if(!(numerator < longest)) {
					numerator -= longest;
					p += da;
				}
				else {
					p += db;
				}
			}
		}

		/// <summary> Draws a Rectangle, calls overloaded method with background as bgColor  </summary>
		/// <param name="pos">Top Left corner of rectangle.</param>
		/// <param name="end">Bottom Right corner of rectangle.</param>
		/// <param name="color">Color to draw with.</param>
		/// <param name="c">Character to use.</param>
		public void Rectangle(Point pos, Point end, int color, ConsoleCharacter c = ConsoleCharacter.Full) {
			Rectangle(pos, end, color, Background, c);
		}

		/// <summary> Draws a Rectangle. </summary>
		/// <param name="pos">Top Left corner of rectangle.</param>
		/// <param name="end">Bottom Right corner of rectangle.</param>
		/// <param name="fgColor">Color to draw with.</param>
		/// <param name="bgColor">Color to draw to the background with.</param>
		/// <param name="c">Character to use.</param>
		public void Rectangle(Point pos, Point end, int fgColor,int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			for(int i = 0; i < end.X - pos.X; i++) {
				SetPixel(new Point(pos.X + i, pos.Y), fgColor,bgColor, c);
				SetPixel(new Point(pos.X + i, end.Y), fgColor, bgColor, c);
			}

			for(int i = 0; i < end.Y - pos.Y + 1; i++) {
				SetPixel(new Point(pos.X, pos.Y + i), fgColor, bgColor, c);
				SetPixel(new Point(end.X, pos.Y + i), fgColor, bgColor, c);
			}
		}

		/// <summary> Draws a Rectangle and fills it, calls overloaded method with background as bgColor </summary>
		/// <param name="a">Top Left corner of rectangle.</param>
		/// <param name="b">Bottom Right corner of rectangle.</param>
		/// <param name="color">Color to draw with.</param>
		/// <param name="c">Character to use.</param>
		public void Fill(Point a, Point b, int color, ConsoleCharacter c = ConsoleCharacter.Full) {
			Fill(a, b, color, Background, c);
		}

		/// <summary> Draws a Rectangle and fills it. </summary>
		/// <param name="a">Top Left corner of rectangle.</param>
		/// <param name="b">Bottom Right corner of rectangle.</param>
		/// <param name="fgColor">Color to draw with.</param>
		/// <param name="bgColor">Color to draw the background with.</param>
		/// <param name="c">Character to use.</param>
		public void Fill(Point a, Point b, int fgColor,int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			for(int y = a.Y; y < b.Y; y++) {
				for(int x = a.X; x < b.X; x++) {
					SetPixel(new Point(x, y), fgColor,bgColor, c);
				}
			}
		}

		/// <summary> Draws a grid, calls overloaded method with background as bgColor </summary>
		/// <param name="a">Top Left corner of grid.</param>
		/// <param name="b">Bottom Right corner of grid.</param>
		/// <param name="spacing">the spacing until next line</param>
		/// <param name="color">Color to draw with.</param>
		/// <param name="c">Character to use.</param>
		public void Grid(Point a, Point b, int spacing, int color, ConsoleCharacter c = ConsoleCharacter.Full) {
			Grid(a, b, spacing, color, Background, c);
		}

		/// <summary> Draws a grid. </summary>
		/// <param name="a">Top Left corner of grid.</param>
		/// <param name="b">Bottom Right corner of grid.</param>
		/// <param name="spacing">the spacing until next line</param>
		/// <param name="fgColor">Color to draw with.</param>
		/// <param name="bgColor">Color to draw the background with.</param>
		/// <param name="c">Character to use.</param>
		public void Grid(Point a, Point b, int spacing, int fgColor,int bgColor, ConsoleCharacter c = ConsoleCharacter.Full)
		{
			for(int y = a.Y; y < b.Y / spacing; y++) {
				Line(new Point(a.X, y * spacing), new Point(b.X, y * spacing), fgColor,bgColor, c);
			}
			for(int x = a.X; x < b.X / spacing; x++) {
				Line(new Point(x * spacing, a.Y), new Point(x * spacing, b.Y), fgColor, bgColor, c);
			}
		}

		/// <summary> Draws a Triangle, calls overloaded method with background as bgColor </summary>
		/// <param name="a">Point A.</param>
		/// <param name="b">Point B.</param>
		/// <param name="c">Point C.</param>
		/// <param name="color">Color to draw with.</param>
		/// <param name="character">Character to use.</param>
		public void Triangle(Point a, Point b, Point c, int color, ConsoleCharacter character = ConsoleCharacter.Full) {
			Triangle(a,b,c,color,Background,character);
		}

		/// <summary> Draws a Triangle. </summary>
		/// <param name="a">Point A.</param>
		/// <param name="b">Point B.</param>
		/// <param name="c">Point C.</param>
		/// <param name="fgColor">Color to draw with.</param>
		/// <param name="bgColor">Color to draw to the background with.</param>
		/// <param name="character">Character to use.</param>
		public void Triangle(Point a, Point b, Point c, int fgColor,int bgColor, ConsoleCharacter character = ConsoleCharacter.Full)
		{
			Line(a, b, fgColor,bgColor, character);
			Line(b, c, fgColor, bgColor, character);
			Line(c, a, fgColor, bgColor, character);
		}

		// Bresenhams Triangle Algorithm

		/// <summary> Draws a Triangle and fills it, calls overloaded method with background as bgColor </summary>
		/// <param name="a">Point A.</param>
		/// <param name="b">Point B.</param>
		/// <param name="c">Point C.</param>
		/// <param name="color">Color to draw with.</param>
		/// <param name="character">Character to use.</param>
		public void FillTriangle(Point a, Point b, Point c, int color, ConsoleCharacter character = ConsoleCharacter.Full) {
			FillTriangle(a, b, c, color, Background, character);
		}

		/// <summary> Draws a Triangle and fills it. </summary>
		/// <param name="a">Point A.</param>
		/// <param name="b">Point B.</param>
		/// <param name="c">Point C.</param>
		/// <param name="fgColor">Color to draw with.</param>
		/// <param name="bgColor">Color to draw to the background with.</param>
		/// <param name="character">Character to use.</param>
		public void FillTriangle(Point a, Point b, Point c, int fgColor, int bgColor, ConsoleCharacter character = ConsoleCharacter.Full)
		{
			Point min = new Point(Math.Min(Math.Min(a.X, b.X), c.X), Math.Min(Math.Min(a.Y, b.Y), c.Y));
			Point max = new Point(Math.Max(Math.Max(a.X, b.X), c.X), Math.Max(Math.Max(a.Y, b.Y), c.Y));

			Point p = new Point();
			for(p.Y = min.Y; p.Y < max.Y; p.Y++) {
				for(p.X = min.X; p.X < max.X; p.X++) {
					int w0 = Orient(b, c, p);
					int w1 = Orient(c, a, p);
					int w2 = Orient(a, b, p);

					if(w0 >= 0 && w1 >= 0 && w2 >= 0) SetPixel(p, fgColor,bgColor, character);
				}
			}
		}

		private int Orient(Point a, Point b, Point c) {
			return ((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X));
		}

		#endregion Primitives

		// Input

		/// <summary>Checks to see if the console is in focus </summary>
		/// <returns>True if Console is in focus</returns>
		private bool ConsoleFocused()
		{
			return NativeMethods.GetConsoleWindow() == NativeMethods.GetForegroundWindow();
		}

		/// <summary> Checks if specified key is pressed. </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>True if key is pressed</returns>
		public bool GetKey(ConsoleKey key) {
			short s = NativeMethods.GetAsyncKeyState((int)key);
			return (s & 0x8000) > 0 && ConsoleFocused();
		}

		/// <summary> Checks if specified keyCode is pressed. </summary>
		/// <param name="virtualkeyCode">keycode to check</param>
		/// <returns>True if key is pressed</returns>
		public bool GetKey(int virtualkeyCode)
		{
			short s = NativeMethods.GetAsyncKeyState(virtualkeyCode);
			return (s & 0x8000) > 0 && ConsoleFocused();
		}

		/// <summary> Checks if specified key is pressed down. </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>True if key is down</returns>
		public bool GetKeyDown(ConsoleKey key) {
			int s = Convert.ToInt32(NativeMethods.GetAsyncKeyState((int)key));
			return (s == -32767) && ConsoleFocused();
		}

		/// <summary> Checks if specified keyCode is pressed down. </summary>
		/// <param name="virtualKeyCode">keycode to check</param>
		/// <returns>True if key is down</returns>
		public bool GetKeyDown(int virtualKeyCode)
		{
			int s = Convert.ToInt32(NativeMethods.GetAsyncKeyState(virtualKeyCode));
			return (s == -32767) && ConsoleFocused();
		}

		/// <summary> Checks if left mouse button is pressed down. </summary>
		/// <returns>True if left mouse button is down</returns>
		public bool GetMouseLeft() {
			short s = NativeMethods.GetAsyncKeyState(0x01);
			return (s & 0x8000) > 0 && ConsoleFocused();
		}

		/// <summary> Checks if right mouse button is pressed down. </summary>
		/// <returns>True if right mouse button is down</returns>
		public bool GetMouseRight()
		{
			short s = NativeMethods.GetAsyncKeyState(0x02);
			return (s & 0x8000) > 0 && ConsoleFocused();
		}

		/// <summary> Checks if middle mouse button is pressed down. </summary>
		/// <returns>True if middle mouse button is down</returns>
		public bool GetMouseMiddle()
		{
			short s = NativeMethods.GetAsyncKeyState(0x04);
			return (s & 0x8000) > 0 && ConsoleFocused();
		}

		/// <summary> Gets the mouse position. </summary>
		/// <returns>The mouse's position in character-space.</returns>
		/// <exception cref="Exception"/>
		public Point GetMousePos() {
			NativeMethods.Rect r = new NativeMethods.Rect();
			NativeMethods.GetWindowRect(consoleHandle, ref r);

			if (NativeMethods.GetCursorPos(out NativeMethods.POINT p)) {
				Point point;
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