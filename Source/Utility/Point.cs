using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGameEngine {
	public struct Point {
		public int X { get; set; }
		public int Y { get; set; }

		public const float Rad2Deg = 180f / (float)Math.PI;
		public const float Deg2Rad = (float)Math.PI / 180f;

		public static Point Zero { get; private set; } = new Point(0, 0);

		public Point(int x, int y) {
			this.X = x;
			this.Y = y;
		}

		public Vector ToVector() {
			return new Vector((float)X, (float)Y);
		}
		public override string ToString() {
			return String.Format("({0}, {1})", X, Y);
		}

		public static Point operator +(Point a, Point b) {
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		public static Point operator -(Point a, Point b) {
			return new Point(a.X - b.X, a.Y - b.Y);
		}

		public static Point operator /(Point a, float b) {
			return new Point((int)(a.X / b), (int)(a.Y / b));
		}
		public static Point operator *(Point a, float b) {
			return new Point((int)(a.X * b), (int)(a.Y * b));
		}

		public static float Distance(Point a, Point b) {
			Point dV = b - a;
			float d = (float)Math.Sqrt(Math.Pow(dV.X, 2) + Math.Pow(dV.Y, 2));
			return d;
		}
		public static float Angle(Point a, Point b) {
			return (float)Math.Atan2(b.Y - a.Y, b.X - a.X) * Rad2Deg;
		}

		public void Clamp(Point min, Point max) {
			X = (X > max.X) ? max.X : X;
			X = (X < min.X) ? min.X : X;

			Y = (Y > max.Y) ? max.Y : Y;
			Y = (Y < min.Y) ? min.Y : Y;
		}
	}
}
