using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameEngine
{
	public struct Glyph
	{
		public char c;
		public int fg;
		public int bg;

		public void set(char c_, int fg_, int bg_) { c = c_; fg = fg_;bg = bg_; }

		public void clear() { c = (char)0;fg = 0;bg = 0; }
	}
}
