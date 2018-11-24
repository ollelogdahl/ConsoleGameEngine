using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameEngine {
	public struct Vector {
		public float X { get; set; }
		public float Y { get; set; }


		public static Vector Zero { get; private set; } = new Vector(0, 0);
		public Vector(float x, float y) {
			this.X = x;
			this.Y = y;
		}

		public Point ToPoint() {
			return new Point((int)Math.Round(X, 0), (int)Math.Round(Y, 0));
		}

		public void Rotate(float a) {
			Vector n = Vector.Zero;

			n.X = (float)(X * Math.Cos(a / 57.3f) - Y * Math.Sin(a / 57.3f));
			n.Y = (float)(X * Math.Sin(a / 57.3f) + Y * Math.Cos(a / 57.3f));

			X = n.X;
			Y = n.Y;
		}

		public static Vector operator + (Vector a, Vector b) {
			return new Vector(a.X + b.X, a.Y + b.Y);
		}
		public static Vector operator - (Vector a, Vector b) {
			return new Vector(a.X - b.X, a.Y - b.Y);
		}

		public static Vector operator / (Vector a, float b) {
			return new Vector((a.X / b), (a.Y / b));
		}
		public static Vector operator * (Vector a, float b) {
			return new Vector((a.X * b), (a.Y * b));
		}

		public static float Distance(Vector a, Vector b) {
			Vector dV = b - a;
			float d = (float)Math.Sqrt(Math.Pow(dV.X, 2) + Math.Pow(dV.Y, 2));
			return d;
		}
	}
}
