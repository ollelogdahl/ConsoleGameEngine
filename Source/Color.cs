namespace ConsoleGameEngine {
	/// <summary> Represents an RGB color. </summary>
	public class Color {
		/// <summary> Red component. </summary>
		public uint R { get; set; }
		/// <summary> Green component. </summary>
		public uint G { get; set; }
		/// <summary> Bkue component. </summary>
		public uint B { get; set; }

		/// <summary> Creates a new Color from rgb. </summary>
		public Color(int r, int g, int b) {
			this.R = (uint)r;
			this.G = (uint)g;
			this.B = (uint)b;
		}
	}
}
