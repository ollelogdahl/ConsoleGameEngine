using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGameEngine {
	/// <summary> Utility class. </summary>
	public class Utility {
		static public int Clamp(int a, int min, int max) {
			a = (a > max) ? max : a;
			a = (a < min) ? min : a;

			return a;
		}
	}
}
