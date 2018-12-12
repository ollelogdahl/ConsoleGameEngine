using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using ConsoleGameEngine;
using System.Threading;

namespace ConsoleGameEngineExamples {
	class Tetris : ConsoleGame {
		readonly Random rand = new Random();
		readonly string[] tetromino = new string[7];

		int[] levels;

		int[] playingField;

		static int fieldWidth = 14; static int fieldHeight = 24;

		int lineCount = 0;
		int frame = 0;
		int currentTetromino = 0;
		int rotation = 0;
		Point current;

		List<int> lines = new List<int>();

		int highscore = 0;
		int score = 0;
		int level = 0;

		bool gameover = false;

		private static void Main(string[] args) {
			new Tetris().Construct(fieldWidth + 2, fieldHeight + 6, 16, 16, FramerateMode.MaxFps);
		}
		public override void Create() {
			Engine.SetPalette(Palettes.Pico8);
			Engine.Borderless();
			Console.Title = "Tetris";
			TargetFramerate = 50;

			tetromino[0] = "..0...0...0...0.";
			tetromino[1] = "..1..11...1.....";
			tetromino[2] = ".....22..22.....";
			tetromino[3] = "..3..33..3......";
			tetromino[4] = ".4...44...4.....";
			tetromino[5] = ".5...5...55.....";
			tetromino[6] = "..6...6..66.....";

			levels = new int[30] { 48, 43, 38, 33, 28, 23, 18, 13, 8, 6, 5, 5, 5, 4, 4, 4, 3, 3, 3,
				2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1};

			Restart();
		}

		public override void Update() {
			if (!gameover) {
				// hanterar input
				if (Engine.GetKeyDown(ConsoleKey.RightArrow)) {
					current.X += DoesPieceFit(currentTetromino, rotation, current + new Point(1, 0)) ? 1 : 0;
				}
				if (Engine.GetKeyDown(ConsoleKey.LeftArrow)) {
					current.X -= DoesPieceFit(currentTetromino, rotation, current - new Point(1, 0)) ? 1 : 0;
				}
				if (Engine.GetKey(ConsoleKey.DownArrow) && frame % 3 == 0) {
					current.Y += DoesPieceFit(currentTetromino, rotation, current + new Point(0, 1)) ? 1 : 0;
				}

				if (Engine.GetKeyDown(ConsoleKey.UpArrow)) {
					rotation += DoesPieceFit(currentTetromino, rotation + 1, current) ? 1 : 0;
				}

				frame++;

				// gör endast denna uppdatering ibland (högre framerate på input)
				if (frame % levels[level] == 0) {
					// tvingar tetrominon neråt
					if (DoesPieceFit(currentTetromino, rotation, current + new Point(0, 1))) {
						current.Y += 1;
					} else {
						// placerar tetrominon på spelfältet
						for (int px = 0; px < 4; px++) {
							for (int py = 0; py < 4; py++) {
								if (tetromino[currentTetromino][Rotate(new Point(px, py), rotation)] != '.') {
									playingField[(current.Y + py) * fieldWidth + (current.X + px)] = currentTetromino + 1;
								}
							}
						}

						// kollar efter rader
						for (int py = 0; py < 4; py++) {
							if (current.Y + py < fieldHeight - 1) {
								bool bline = true;
								for (int px = 1; px < fieldWidth - 1; px++) bline &= (playingField[(current.Y + py) * fieldWidth + px]) != 0;

								if (bline) {
									// ta bort linje
									for (int px = 1; px < fieldWidth - 1; px++) {
										playingField[(current.Y + py) * fieldWidth + px] = 8;       // 8 för animation :)
									}

									lines.Add(current.Y + py);
								}
							}
						}

						// gravitation av mappen ifall man gör en rad
						if (lines.Any()) {
							for (int line = 0; line < lines.Count; line++) {
								for (int x = 1; x < fieldWidth - 1; x++) {
									// börja nerifrån
									for (int y = lines[line]; y > 0; y--) {
										if (y - 1 > 0) playingField[y * fieldWidth + x] = playingField[(y - 1) * fieldWidth + x];
										else playingField[y * fieldWidth + x] = 0;
									}
								}
							}

							lineCount += lines.Count;
							switch(lines.Count) {
								case 1: score += 40   * (level + 1); break;
								case 2: score += 100  * (level + 1); break;
								case 3: score += 300  * (level + 1); break;
								case 4: score += 1200 * (level + 1); break;
							}
						}
						lines.Clear();

						current.X = fieldWidth / 2 -2;
						current.Y = 0;
						rotation = 0;
						currentTetromino = rand.Next(0, tetromino.Length);

						if(lineCount > 10) {
							lineCount = 0;
							level++;
						}

						// om tetrominon inte får plats direkt förlorar spelaren
						gameover = !DoesPieceFit(currentTetromino, rotation, current);
					}
				}

			} else {
				Engine.Fill(new Point(fieldWidth / 2 - 5, fieldHeight / 2 - 1), new Point(fieldWidth / 2 + 6, fieldHeight / 2 + 3), 0, ConsoleCharacter.Full);
				Engine.WriteText(new Point(fieldWidth / 2 - 4, fieldHeight/2), "Game Over!", 8);
				Engine.WriteText(new Point(fieldWidth / 2 - 4, fieldHeight / 2 + 1), "Highscore:", 7);

				if(highscore < score) {
					highscore = score;
				}

				Engine.WriteText(new Point(fieldWidth / 2 - 4, fieldHeight / 2 + 2), highscore.ToString(), 9);

				Engine.Frame(new Point(fieldWidth / 2 - 5, fieldHeight / 2 - 1), new Point(fieldWidth / 2 + 6, fieldHeight / 2 + 3), 7);

				Engine.DisplayBuffer();

				Thread.Sleep(4000);
				Restart();
			}
		}

		public override void Render() {
			Engine.ClearBuffer();

			// ritar banan
			for(int x = 0; x < fieldWidth; x++) {
				for(int y = 0; y < fieldHeight; y++) {
					if(playingField[(y) * fieldWidth + x] != 0) Engine.SetPixel(new Point(x+1, y+1), playingField[y * fieldWidth + x] + 7);
				}
			}

			// ritar nuvarande tetromino
			for(int px = 0; px < 4; px ++) {
				for(int py = 0; py < 4; py++) {
					if(tetromino[currentTetromino][Rotate(new Point(px, py), rotation)] != '.') {
						Engine.SetPixel(new Point(current.X + px+1, current.Y + py+1), GetTetrominoColor(currentTetromino) + 8);
					}
				}
			}
			Engine.Frame(new Point(1, 0), new Point(fieldWidth, fieldHeight), 7);
			Engine.WriteText(new Point(2, fieldHeight+1), "Score", 7);
			Engine.WriteText(new Point(fieldWidth - score.ToString("N0").Count(), fieldHeight+1), score.ToString("N0"), 9);
			Engine.WriteText(new Point(2, fieldHeight + 2), "Line", 7);
			Engine.WriteText(new Point(fieldWidth - score.ToString().Count(), fieldHeight + 2), lineCount.ToString(), 7);
			Engine.WriteText(new Point(2, fieldHeight + 3), "Level", 7);
			Engine.WriteText(new Point(fieldWidth - score.ToString().Count(), fieldHeight + 3), level.ToString(), 7);

			Engine.DisplayBuffer();
		}



		int Rotate(Point p, int r) {
			int i = 0;
			switch(r % 4) {
				case 0:		// 0 grader
					i = p.Y * 4 + p.X;
					break;
				case 1:     // 90 grader
					i = 12 + p.Y - (p.X * 4);
					break;
				case 2:     // 180 grader
					i = 15 - (p.Y * 4) - p.X;
					break;
				case 3:     // 270 grader
					i = 3 - p.Y + (p.X * 4);
					break;
			}
			return i;
		}

		bool DoesPieceFit(int selTetromino, int rot, Point pos) {
			for(int px = 0; px < 4; px++) {
				for(int py = 0; py < 4; py++) {
					int pieceIndex = Rotate(new Point(px, py), rot);

					int fieldIndex = (pos.Y + py) * fieldWidth + (pos.X + px);

					if(	pos.X + px >= 0 && pos.X + px < fieldWidth &&
						pos.Y + py >= 0 && pos.Y + py < fieldHeight) {
						if (tetromino[selTetromino][pieceIndex] != '.' && playingField[fieldIndex] != 0) return false;
					}
				}
			}

			return true;
		}

		int GetTetrominoColor(int t) {
			Match m = Regex.Match(tetromino[t], @"\d");
			return Convert.ToInt32(m.Value[0]);
		}

		void Restart() {
			score = 0;
			gameover = false;

			playingField = new int[fieldWidth * fieldHeight];
			for (int x = 0; x < fieldWidth; x++) // kant för banan
				for (int y = 0; y < fieldHeight; y++)
					playingField[y * fieldWidth + x] = (x == 0 || x == fieldWidth - 1 || y == fieldHeight - 1) ? -1 : 0;    // väggar

			current = new Point(fieldWidth / 2 - 2, 0);
			currentTetromino = rand.Next(0, tetromino.Length);
		}
	}
}
