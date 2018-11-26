using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleGameEngine;

namespace ConsoleGameEngineExamples {
	class Example3D : ConsoleGame {
		static void Main(string[] args) {
			new Example3D().Construct(256, 256, 2, 2, FramerateMode.Unlimited);
		}
		readonly Random rand = new Random();

		Mesh mesh;
		Matrix projectionMatrix;
		Vec3D modelPosition = new Vec3D(0, 0, 4);

		Vec3D camera = new Vec3D(0, 0, 0);
		Vec3D lookDirection = new Vec3D(0, 0, 0);
		Vec3D lightDirection = new Vec3D(0.0f, 0.0f, -1.0f);

		List<Triangle> trianglesToRaster = new List<Triangle>();

		Point lastMousePos = Point.Zero;


		private bool drawWireframe = false;
		private bool drawSolid = true;

		private float yRot = 0;
		private float xRot = 0;

		private int consoleHeight;
		private int consoleWidth;

		public override void Create() {
			Engine.SetPalette(Palettes.Pico8);
			Engine.SetBackground(0);
			Engine.Borderless(true);

			Console.Title = "3D Demo";

			mesh.LoadFromObj("cube.obj");

			consoleWidth = Engine.WindowSize.X;
			consoleHeight = Engine.WindowSize.Y;

			// projektions matris
			float near = 0.1f;
			float far = 1000.0f;
			float fov = 90.0f;
			float aspectRatio = (float)consoleHeight / (float)consoleWidth;
			projectionMatrix = Matrix.ProjectionMatrix(fov, aspectRatio, near, far);

			// normaliserar ljusriktningen
			lightDirection.Normalize();
		}

		public override void Update() {
			if (Engine.GetKeyDown(ConsoleKey.Y)) {
				drawWireframe = !drawWireframe;
			}
			if (Engine.GetKeyDown(ConsoleKey.H)) {
				drawSolid = !drawSolid;
			}

			if(Engine.GetMouseLeft()) {
				Point delta = Engine.GetMousePos() - lastMousePos;
				yRot += delta.X / 80f;
				xRot += delta.Y / 80f;
			}

			lastMousePos = Engine.GetMousePos();

			// matriserar
			Matrix transformMat, rotationMat;
			rotationMat = Matrix.RotationMatrixY(yRot);
			rotationMat *= Matrix.RotationMatrixX(xRot);
			transformMat = Matrix.Translation(modelPosition);

			// generera trianglar
			for (int i = 0; i < mesh.Triangles.Length; i++) {
				Triangle vertex = mesh.Triangles[i];
				Triangle rotated = vertex.MatMul(rotationMat);

				Triangle transformed = rotated.MatMul(transformMat);

				// beräknar kryssprodukt av line1 och line2 för att hitta normal.
				Vec3D normal, line1, line2;
				line1 = transformed.p[1] - transformed.p[0];
				line2 = transformed.p[2] - transformed.p[0];

				normal = Vec3D.Cross(line1, line2);
				normal.Normalize();

				// testar ifall vi kan se ytan
				if (Vec3D.Dot(normal, transformed.p[0] - camera) < 0.0f) {
					// beräknar ljus
					float l = Vec3D.Dot(lightDirection, normal);
					ConsoleCharacter character = ConsoleCharacter.Light;
					if (l > 0.4) character = ConsoleCharacter.Medium;
					if (l > 0.7) character = ConsoleCharacter.Dark;
					if (l > 1) character = ConsoleCharacter.Full;

					// projekterar från 3D -> 2D
					Triangle projected = new Triangle(null);
					projected = transformed.MatMul(projectionMatrix);

					// transformerar och skalar projektionen
					Vec3D offsetView = new Vec3D(1, 1, 0);
					projected.p[0] += offsetView;
					projected.p[1] += offsetView;
					projected.p[2] += offsetView;

					projected.p[0].x *= 0.5f * consoleWidth; projected.p[0].y *= 0.5f * consoleHeight;
					projected.p[1].x *= 0.5f * consoleWidth; projected.p[1].y *= 0.5f * consoleHeight;
					projected.p[2].x *= 0.5f * consoleWidth; projected.p[2].y *= 0.5f * consoleHeight;

					projected.c = character;
					trianglesToRaster.Add(projected);
				}
			}

			// sortera
			trianglesToRaster.Sort((t1, t2) => ((t2.p[0].z + t2.p[1].z + t2.p[2].z).CompareTo((t1.p[0].z + t1.p[1].z + t1.p[2].z))));
		}

		public override void Render() {
			Engine.ClearBuffer();


			foreach(Triangle t in trianglesToRaster) {
				Point a = new Point((int)t.p[0].x, (int)t.p[0].y);
				Point b = new Point((int)t.p[1].x, (int)t.p[1].y);
				Point c = new Point((int)t.p[2].x, (int)t.p[2].y);
				if(drawSolid) Engine.FillTriangle(b, a, c, 9, t.c);
				if(drawWireframe) Engine.Triangle(b, a, c, 7, t.c);
			}

			trianglesToRaster.Clear();

			Engine.DisplayBuffer();
		}
	}

	public struct Vec3D {
		public float x, y, z, w;

		public Vec3D(float a, float b, float c) {
			x = a;
			y = b;
			z = c;
			w = 0;
		}

		public void Normalize() {
			float l = (float)Math.Sqrt(x * x + y * y + z * z);
			x /= l; y /= l; z /= l;
		}

		// skalärprodukt https://sv.wikipedia.org/wiki/Skalärprodukt
		public static float Dot(Vec3D a, Vec3D b) {
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		public static Vec3D Cross(Vec3D a, Vec3D b) {
			Vec3D n = new Vec3D {
				x = a.y * b.z - a.z * b.y,
				y = a.z * b.x - a.x * b.z,
				z = a.x * b.y - a.y * b.x
			};

			return n;
		}

		public static Vec3D operator +(Vec3D a, Vec3D b) {
			return new Vec3D(a.x + b.x, a.y + b.y, a.z + b.z);
		}
		public static Vec3D operator +(Vec3D a, float b) {
			return new Vec3D(a.x + b, a.y + b, a.z + b);
		}

		public static Vec3D operator -(Vec3D a, Vec3D b) {
			return new Vec3D(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vec3D operator *(Vec3D a, float b) {
			return new Vec3D(a.x * b, a.y * b, a.z * b);
		}

	}
	public struct Triangle {
		public Vec3D[] p;

		public int color;
		public ConsoleCharacter c;

		public Triangle(Vec3D pa, Vec3D pb, Vec3D pc) {
			p = new Vec3D[3];
			p[0] = pa;
			p[1] = pb;
			p[2] = pc;

			color = 0;
			c = ConsoleCharacter.Null;
		}

		public Triangle(object n) {
			p = new Vec3D[3];
			color = 0;
			c = ConsoleCharacter.Null;
		}

		public void Translate(Vec3D delta) {
			p[0] += delta;
			p[1] += delta;
			p[2] += delta;
		}

		public Triangle MatMul(Matrix m) {
			Triangle t = new Triangle(null);
			t.p[0] = Matrix.MultiplyVector(p[0], m);
			t.p[1] = Matrix.MultiplyVector(p[1], m);
			t.p[2] = Matrix.MultiplyVector(p[2], m);

			return t;
		}
	}

	public struct Mesh {
		public Vec3D[] Vertices { get; set; }
		public Triangle[] Triangles { get; set; }

		public bool LoadFromObj(string filename) {
			StreamReader s = new StreamReader(filename);

			List<Vec3D> verts = new List<Vec3D>();
			List<Triangle> tris = new List<Triangle>();
			
			while(!s.EndOfStream) {
				string line = s.ReadLine();

				if(line[0] == 'v') {
					Vec3D v = new Vec3D();
					string[] str = line.Replace('.', ',').Split();
					v.x = float.Parse(str[1]);
					v.y = float.Parse(str[2]);
					v.z = float.Parse(str[3]);
					verts.Add(v);
				}
				if(line[0] == 'f') {
					Triangle t = new Triangle();
					string[] str = line.Split();
					t.p = new Vec3D[3];
					t.color = 6;
					t.p[0] = verts[Convert.ToInt32(str[1]) - 1];
					t.p[1] = verts[Convert.ToInt32(str[2]) - 1];
					t.p[2] = verts[Convert.ToInt32(str[3]) - 1];
					tris.Add(t);
				}
			}

			Vertices = verts.ToArray();
			Triangles = tris.ToArray();

			return true;
		}
	}

	public class Matrix {
		public float[,] m = new float[4, 4];

		public static Matrix operator *(Matrix a, Matrix b) {
			Matrix mat = new Matrix();
			for(int c = 0; c < 4; c++) {
				for(int r = 0; r < 4; r++) {
					mat.m[r, c] = a.m[r, 0] * b.m[0, c] + a.m[r, 1] * b.m[1, c] + a.m[r, 2] * b.m[2, c] + a.m[r, 3] * b.m[3, c];
				}
			}
			return mat;
		}

		public static Vec3D MultiplyVector(Vec3D i, Matrix m) {
			Vec3D v = new Vec3D();
			float w = 0;
			v.x = i.x * m.m[0, 0] + i.y * m.m[1, 0] + i.z * m.m[2, 0] + m.m[3, 0];
			v.y = i.x * m.m[0, 1] + i.y * m.m[1, 1] + i.z * m.m[2, 1] + m.m[3, 1];
			v.z = i.x * m.m[0, 2] + i.y * m.m[1, 2] + i.z * m.m[2, 2] + m.m[3, 2];
			  w = i.x * m.m[0, 3] + i.y * m.m[1, 3] + i.z * m.m[2, 3] + m.m[3, 3];

			if (w != 0.0f) {
				v.x /= w; v.y /= w; v.z /= w;
			}

			return v;
		}

		public static Matrix Translation(Vec3D t) {
			Matrix mat = new Matrix();
			mat.m[0, 0] = 1.0f;
			mat.m[1, 1] = 1.0f;
			mat.m[2, 2] = 1.0f;
			mat.m[3, 3] = 1.0f;
			mat.m[3, 0] = t.x;
			mat.m[3, 1] = t.y;
			mat.m[3, 2] = t.z;
			return mat;
		}

		public static Matrix Identity() {
			Matrix mat = new Matrix();
			mat.m[0, 0] = 1.0f;	mat.m[0, 1] = 1.0f;	mat.m[0, 2] = 1.0f;	mat.m[0, 3] = 1.0f;
			mat.m[1, 0] = 1.0f; mat.m[1, 1] = 1.0f; mat.m[1, 2] = 1.0f; mat.m[1, 3] = 1.0f;
			mat.m[2, 0] = 1.0f; mat.m[2, 1] = 1.0f; mat.m[2, 2] = 1.0f; mat.m[2, 3] = 1.0f;
			mat.m[3, 0] = 1.0f; mat.m[3, 1] = 1.0f; mat.m[3, 2] = 1.0f; mat.m[3, 3] = 1.0f;
			return mat;
		}

		public static Matrix PointAtMatrix(Vec3D pos, Vec3D target, Vec3D up) {
			// ny frammåt
			Vec3D newForward = target - pos;
			newForward.Normalize();

			// ny uppåt
			Vec3D a = newForward * Vec3D.Dot(up, newForward);
			Vec3D newUp = up - a;
			newUp.Normalize();

			Vec3D newRight = Vec3D.Cross(newUp, newForward);
			Matrix mat = new Matrix();
			mat.m[0, 0] = newRight.x;	mat.m[0, 1] = newRight.y;	mat.m[0, 2] = newRight.z;	mat.m[0, 3] = 0.0f;
			mat.m[1, 0] = newUp.x;		mat.m[1, 1] = newUp.y;		mat.m[1, 2] = newUp.z;		mat.m[1, 3] = 0.0f;
			mat.m[2, 0] = newForward.x; mat.m[2, 1] = newForward.y; mat.m[2, 2] = newForward.z; mat.m[2, 3] = 0.0f;
			mat.m[3, 0] = pos.x;		mat.m[2, 1] = pos.y;		mat.m[2, 2] = pos.z;		mat.m[2, 3] = 1.0f;
			return mat;
		}
		public static Matrix ProjectionMatrix(float fov, float aspect, float near, float far) {
			Matrix mat = new Matrix();
			float fovRad = 1.0f / (float)Math.Tan(fov * 0.5f / (180 / (float)Math.PI));
			mat.m[0, 0] = aspect * fovRad;
			mat.m[1, 1] = fovRad;
			mat.m[2, 2] = far / (far - near);
			mat.m[3, 2] = (-far * near) / (far - near);
			mat.m[2, 3] = 1.0f;
			mat.m[3, 3] = 0.0f;

			return mat;
		}

		// Endast för rotation/translations matriser
		public static Matrix QuickInverse(Matrix m) {
			Matrix matrix = new Matrix();
			matrix.m[0, 0] = m.m[0, 0]; matrix.m[0, 1] = m.m[1, 0]; matrix.m[0, 2] = m.m[2, 0]; matrix.m[0, 3] = 0.0f;
			matrix.m[1, 0] = m.m[0, 1]; matrix.m[1, 1] = m.m[1, 1]; matrix.m[1, 2] = m.m[2, 1]; matrix.m[1, 3] = 0.0f;
			matrix.m[2, 0] = m.m[0, 2]; matrix.m[2, 1] = m.m[1, 2]; matrix.m[2, 2] = m.m[2, 2]; matrix.m[2, 3] = 0.0f;
			matrix.m[3, 0] = -(m.m[3, 0] * matrix.m[0, 0] + m.m[3, 1] * matrix.m[1, 0] + m.m[3, 2] * matrix.m[2, 0]);
			matrix.m[3, 1] = -(m.m[3, 0] * matrix.m[0, 1] + m.m[3, 1] * matrix.m[1, 1] + m.m[3, 2] * matrix.m[2, 1]);
			matrix.m[3, 2] = -(m.m[3, 0] * matrix.m[0, 2] + m.m[3, 1] * matrix.m[1, 2] + m.m[3, 2] * matrix.m[2, 2]);
			matrix.m[3, 3] = 1.0f;
			return matrix;
		}

		public static Matrix RotationMatrixX(float fAngleRad) {
			Matrix mat = new Matrix();
			mat.m[0, 0] = 1.0f;
			mat.m[1, 1] = (float)Math.Cos(fAngleRad);
			mat.m[1, 2] = (float)Math.Sin(fAngleRad);
			mat.m[2, 1] = (float)-Math.Sin(fAngleRad);
			mat.m[2, 2] = (float)Math.Cos(fAngleRad);
			mat.m[3, 3] = 1.0f;
			return mat;
		}

		public static Matrix RotationMatrixY(float fAngleRad) {
			Matrix mat = new Matrix();
			mat.m[0, 0] = (float)Math.Cos(fAngleRad);
			mat.m[0, 2] = (float)Math.Sin(fAngleRad);
			mat.m[2, 0] = (float)-Math.Sin(fAngleRad);
			mat.m[1, 1] = 1.0f;
			mat.m[2, 2] = (float)Math.Cos(fAngleRad);
			mat.m[3, 3] = 1.0f;
			return mat;
		}

		public static Matrix RotationMatrixZ(float fAngleRad) {
			Matrix mat = new Matrix();
			mat.m[0, 0] = (float)Math.Cos(fAngleRad);
			mat.m[0, 1] = (float)Math.Sin(fAngleRad);
			mat.m[1, 0] = (float)-Math.Sin(fAngleRad);
			mat.m[1, 1] = (float)Math.Cos(fAngleRad);
			mat.m[2, 2] = 1.0f;
			mat.m[3, 3] = 1.0f;
			return mat;
		}
	}
}
