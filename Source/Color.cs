namespace ConsoleGameEngine {
	public class Color {
		public uint R { get; set; }
		public uint G { get; set; }
		public uint B { get; set; }

		public Color(int r, int g, int b) {
			this.R = (uint)r;
			this.G = (uint)g;
			this.B = (uint)b;
		}
	}
}
