using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleGameEngine {
	/// <summary> A FIGlet font. </summary>
	public class FigletFont {

		public int BaseLine { get; private set; }
		public int CodeTagCount { get; private set; }
		public int CommentLines { get; private set; }
		public int FullLayout { get; private set; }
		public string HardBlank { get; private set; }
		public int Height { get; private set; }
		public int Kerning { get; private set; }
		public string[] Lines { get; private set; }
		public int MaxLength { get; private set; }
		public int OldLayout { get; private set; }
		public int PrintDirection { get; private set; }
		public string Signature { get; private set; }

		public static FigletFont Load(string filePath) {
			if (filePath == null) throw new ArgumentNullException(nameof(filePath));
			IEnumerable<string> fontLines = File.ReadLines(filePath);

			FigletFont font = new FigletFont() {
				Lines = fontLines.ToArray()
			};
			var cs = font.Lines.First();
			var configs = cs.Split(' ');
			font.Signature = configs.First().Remove(configs.First().Length - 1);

			if(font.Signature == "flf2a") {
				font.HardBlank = configs.First().Last().ToString();
				font.Height = ParseInt(configs, 1);
				font.BaseLine = ParseInt(configs, 2);
				font.MaxLength = ParseInt(configs, 3);
				font.OldLayout = ParseInt(configs, 4);
				font.CommentLines = ParseInt(configs, 5);
				font.PrintDirection = ParseInt(configs, 6);
				font.FullLayout = ParseInt(configs, 7);
				font.CodeTagCount = ParseInt(configs, 8);
			}


			return font;
		}

		private static int ParseInt(string[] values, int index) {
			var i = 0;
			if(values.Length > index) {
				int.TryParse(values[index], out i);
			}

			return i;
		}



		// ----
		internal static int GetStringWidth(FigletFont font, string value) {
			List<int> charWidths = new List<int>();
			foreach (var character in value) {
				int charWidth = 0;
				for (int line = 1; line <= font.Height; line++) {
					string figletCharacter = GetCharacter(font, character, line);

					charWidth = figletCharacter.Length > charWidth ? figletCharacter.Length : charWidth;
				}
				charWidths.Add(charWidth);
			}

			return charWidths.Sum();
		}

		internal static string GetCharacter(FigletFont font, char character, int line) {
			var start = font.CommentLines + ((Convert.ToInt32(character) - 32) * font.Height);
			var result = font.Lines[start + line];
			var lineEnding = result[result.Length - 1];
			result = Regex.Replace(result, @"\" + lineEnding + "{1,2}$", string.Empty);

			if (font.Kerning > 0) {
				result += new string(' ', font.Kerning);
			}

			return result.Replace(font.HardBlank, " ");
		}
	}
}
