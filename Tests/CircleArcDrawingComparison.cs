using ConsoleGameEngine;

namespace ConsoleGameEngineTests
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
	        var consoleEngine = new ConsoleEngine(60, 45, 8, 8);
			while (true)
			{
				consoleEngine.WriteText(new Point(05, 0), "New", 3, 4);
				consoleEngine.Sector(new Point(05, 5), 4,180, 270 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.Sector(new Point(15, 5), 4,360, 0 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.Sector(new Point(25, 5), 4,0, 360 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.Sector(new Point(35, 5), 4,270 , 90, 4, 13, ConsoleCharacter.Full);
				consoleEngine.Sector(new Point(45, 5), 4, -90 , 135, 4, 13, ConsoleCharacter.Full);
				consoleEngine.Sector(new Point(55, 5), 4, 90 , 270, 4, 13, ConsoleCharacter.Full);
				
				consoleEngine.WriteText(new Point(05, 10), "Old", 3, 4);
				consoleEngine.SemiCircle(new Point(05, 15), 4,180, 270 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.SemiCircle(new Point(15, 15), 4,360, 0 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.SemiCircle(new Point(25, 15), 4,0, 360 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.SemiCircle(new Point(35, 15), 4,270 , 90 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.SemiCircle(new Point(45, 15), 4,-90 , 135 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.SemiCircle(new Point(55, 15), 4,90 , 270 , 4, 13, ConsoleCharacter.Full);
				
				
				consoleEngine.WriteText(new Point(05, 20), "New", 3, 4);
				consoleEngine.Arc2(new Point(05, 25), 4,180, 270 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.Arc2(new Point(15, 25), 4,360, 0 , 4, 13, ConsoleCharacter.Full);
				consoleEngine.Arc2(new Point(25, 25), 4,0, 360 , 4, 13, ConsoleCharacter.Full);

				consoleEngine.WriteText(new Point(05, 30), "Old", 3, 4);
				consoleEngine.Arc(new Point(05, 35), 4, 4,13,270, ConsoleCharacter.Full);
				consoleEngine.Arc(new Point(15, 35), 4, 4,13,0, ConsoleCharacter.Full);
				consoleEngine.Arc(new Point(25, 35), 4, 4,13,360, ConsoleCharacter.Full);

				consoleEngine.Circle(new Point(45, 25), 4, 4, 13, ConsoleCharacter.Full);
				
				consoleEngine.DisplayBuffer();
			}
        }
    }
}