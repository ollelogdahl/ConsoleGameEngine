namespace ConsoleGameEngine {
	using System;
	using System.Linq;
	using System.Threading;

	/*
	 * ConsoleGame
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 */
	public abstract class ConsoleGame {
		public ConsoleEngine Engine { get; private set; }

		public int FrameCounter { get; set; }
		public float DeltaTime { get; set; }

		public int TargetFramerate { get; set; }

		private bool Running { get; set; }
		private Thread gameThread;

		private double[] framerateSamples;

		public void Construct(int width, int height, int fontW, int fontH, FramerateMode m) {
			TargetFramerate = 30;

			Engine = new ConsoleEngine(width, height, fontW, fontH);
			Create();

			if(m == FramerateMode.Unlimited) gameThread = new Thread(new ThreadStart(GameLoopUnlimited));
			if (m == FramerateMode.MaxFps) gameThread = new Thread(new ThreadStart(GameLoopLocked));
			Running = true;
			gameThread.Start();

			// gör special checks som ska gå utanför spelloopen
			// om spel-loopen hänger sig ska man fortfarande kunna avsluta
			while(Running) {
				CheckForExit();
			}
		}

		private void GameLoopLocked() {
			int sampleCount = TargetFramerate;
			framerateSamples = new double[sampleCount];

			DateTime lastTime;
			float uncorrectedSleepDuration = 1000 / TargetFramerate;
			while (Running) {
				lastTime = DateTime.UtcNow;

				FrameCounter++;
				FrameCounter = FrameCounter % sampleCount;


				// kör main programmet
				Update();
				Render();

				float computingDuration = (float)(DateTime.UtcNow - lastTime).TotalMilliseconds;
				int sleepDuration = (int)(uncorrectedSleepDuration - computingDuration);
				if (sleepDuration > 0) {
					// programmet ligger före maxFps, sänker det
					Thread.Sleep(sleepDuration);
				}

				// beräknar framerate
				TimeSpan diff = DateTime.UtcNow - lastTime;
				DeltaTime = (float)(1 / (TargetFramerate * diff.TotalSeconds));

				framerateSamples[FrameCounter] = (double)diff.TotalSeconds;
			}
		}
		private void GameLoopUnlimited() {
			int sampleCount = TargetFramerate;
			framerateSamples = new double[sampleCount];

			DateTime lastTime;
			while(Running) {
				lastTime = DateTime.UtcNow;

				FrameCounter++;
				FrameCounter = FrameCounter % sampleCount;

				Update();
				Render();

				// beräknar framerate
				TimeSpan diff = DateTime.UtcNow - lastTime;
				DeltaTime = (float)diff.TotalSeconds;

				framerateSamples[FrameCounter] = diff.TotalSeconds;

				// kollar om spelaren vill sluta
				CheckForExit();
			}
		}

		public double GetFramerate() {
			return 1 / (framerateSamples.Sum() / (TargetFramerate));
		}
		private void CheckForExit() {
			if(Engine.GetKeyDown(ConsoleKey.Delete)) {
				Running = false;
			}
		}

		public abstract void Create();
		public abstract void Update();
		public abstract void Render();
	}
}
