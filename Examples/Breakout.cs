using ConsoleGameEngine;
using System;

namespace ConsoleGameEngineExamples
{
    internal struct Block
    {
        internal int x;
        internal int y;
        internal bool alive;
        internal int color;

        internal Block(int xpos, int ypos, bool a, int c)
        {
            x = xpos;
            y = ypos;
            alive = a;
            color = c;
        }
    }
    internal class Breakout : ConsoleGame
    {
        static int fieldWidth = 24;
        static int fieldHeight = 30;
        static int ballDirectionUpRight = 0;
        static int ballDirectionUpLeft = 1;
        static int ballDirectionDownRight = 2;
        static int ballDirectionDownLeft = 3;
        static int blockRows = 8;
        static int blockCols = 6;
        static int gameStateStart = 0;
        static int gameStatePlaying = 1;
        static int gameStateEnd = 2;
        static int startingBlockY = 2;

        static int startingX = 8;
        static int startingY = 23;
        static int startingBallX = 12;
        static int startingBallY = 22;
        static int defaultBallWidth = 1;
        static int defaultBallHeight = 1;
        static int startingBallXVel = 1;
        static int startingBallYVel = 1;
        static int startingBallMoveDelay = 4;
        static int startingBallDirection = ballDirectionUpRight;
        static int defaultPaddleWidth = 4;
        static int defaultPaddleHeight = 1;
        static int startingTurns = 3;
        static int startingGameState = gameStateStart;

        int paddlex;
        int paddley;
        int width;
        int height;

        int ballx;
        int bally;
        int ballVelx;
        int ballVely;
        int ballWidth;
        int ballHeight;
        int ballDirection;
        int ballMoveCounter;
        int ballMoveDelay;
        Block[,] blocks;
        int score;
        int turns;
        int gameState;
        string gameOverText;

        private static void Main(string[] args)
        {
            new Breakout().Construct(fieldWidth, fieldHeight, 16, 16, FramerateMode.MaxFps);
        }

        public override void Create()
        {
            Engine.SetPalette(Palettes.Pico8);
            Console.Title = "Breakout";
            TargetFramerate = 50;
            blocks = new Block[blockRows, blockCols];
            width = defaultPaddleWidth;
            height = defaultPaddleHeight;
            gameOverText = "";
            int color = 8;
            for (int y = 0; y < blockRows; y++)
            {
                for (int x = 0; x < blockCols; x++)
                {
                    blocks[y, x] = new Block(x * width, (y * height) + startingBlockY, false, color);
                }
                color++;
            }
            gameState = startingGameState;
        }

        public override void Render()
        {
            Engine.ClearBuffer();

            if (gameState == gameStateStart)
            {
                Engine.WriteText(new Point(1, 4), "Breakout", 7);
                Engine.WriteText(new Point(1, 8), "Press Enter to Play", 7);
                Engine.WriteText(new Point(1, 12), "Press Del to Exit", 7);


            }
            else if (gameState == gameStatePlaying)
            {
                Engine.Fill(new Point(paddlex, paddley), new Point(paddlex + width, paddley + height), 7);

                Engine.Fill(new Point(ballx, bally), new Point(ballx + ballWidth, bally + ballHeight), 7);

                for (int y = 0; y < blockRows; y++)
                {
                    for (int x = 0; x < blockCols; x++)
                    {
                        if (blocks[y, x].alive)
                        {
                            Engine.Fill(new Point(blocks[y, x].x, blocks[y, x].y), new Point(blocks[y, x].x + width, blocks[y, x].y + height), blocks[y, x].color);
                        }
                    }
                }

                Engine.WriteText(new Point(1, fieldHeight - 4), "Score: " + score, 7);
                Engine.WriteText(new Point(1, fieldHeight - 2), "Turns: " + turns, 7);
            }
            else if (gameState == gameStateEnd)
            {
                Engine.WriteText(new Point(1, 4), gameOverText, 7);
                Engine.WriteText(new Point(1, 8), "Final Score: " + score, 7);
                Engine.WriteText(new Point(1, 12), "Press Enter to Play", 7);
                Engine.WriteText(new Point(1, 16), "Press del to Exit", 7);
            }
            Engine.DisplayBuffer();
        }

        public override void Update()
        {
            if (gameState == gameStateStart)
            {
                if (Engine.GetKeyDown(ConsoleKey.Enter))
                {
                    reset();
                    gameState = gameStatePlaying;
                }
            }
            if (gameState == gameStatePlaying)
            {
                // hanterar input
                if (Engine.GetKeyDown(ConsoleKey.RightArrow))
                {
                    paddlex++;
                    if (paddlex > (fieldWidth - width))
                    {
                        paddlex = fieldWidth - width;
                    }

                }
                if (Engine.GetKeyDown(ConsoleKey.LeftArrow))
                {
                    paddlex--;
                    if (paddlex < 0) paddlex = 0;
                }

                // Update ball
                ballMoveCounter++;
                if (ballMoveCounter > ballMoveDelay)
                {
                    if (ballDirection == ballDirectionUpLeft)
                    {
                        bally -= ballVely;
                        ballx -= ballVelx;
                    }
                    else if (ballDirection == ballDirectionUpRight)
                    {
                        bally -= ballVely;
                        ballx += ballVelx;
                    }
                    else if (ballDirection == ballDirectionDownLeft)
                    {
                        bally += ballVely;
                        ballx -= ballVelx;
                    }
                    else if (ballDirection == ballDirectionDownRight)
                    {
                        bally += ballVely;
                        ballx += ballVelx;
                    }

                    // Collide top
                    if (bally < 0)
                    {
                        bally = 0;
                        if (ballDirection == ballDirectionUpLeft)
                            ballDirection = ballDirectionDownLeft;
                        else if (ballDirection == ballDirectionUpRight)
                            ballDirection = ballDirectionDownRight;
                    }

                    if (collidePaddle())
                    {
                        bally -= ballVely;
                        if (ballDirection == ballDirectionDownLeft)
                            ballDirection = ballDirectionUpLeft;
                        else if (ballDirection == ballDirectionDownRight)
                            ballDirection = ballDirectionUpRight;
                    }

                    if (collideBlocks())
                    {
                        if (ballDirection == ballDirectionDownLeft)
                        {
                            ballDirection = ballDirectionUpLeft;
                        }
                        else if (ballDirection == ballDirectionDownRight)
                        {
                            ballDirection = ballDirectionUpRight;
                        }
                        else if (ballDirection == ballDirectionUpLeft)
                        {
                            ballDirection = ballDirectionDownLeft;
                        }
                        else if (ballDirection == ballDirectionUpRight)
                        {
                            ballDirection = ballDirectionDownRight;
                        }
                    }

                    // collide left
                    if (ballx < 0)
                    {
                        ballx = 0;
                        if (ballDirection == ballDirectionDownLeft)
                        {
                            ballDirection = ballDirectionDownRight;
                        }
                        else if (ballDirection == ballDirectionDownRight)
                        {
                            ballDirection = ballDirectionDownLeft;
                        }
                        else if (ballDirection == ballDirectionUpLeft)
                        {
                            ballDirection = ballDirectionUpRight;
                        }
                        else if (ballDirection == ballDirectionUpRight)
                        {
                            ballDirection = ballDirectionUpLeft;
                        }
                    }

                    // collide right
                    if (ballx >= fieldWidth - ballWidth)
                    {
                        ballx = fieldWidth - ballWidth;
                        if (ballDirection == ballDirectionDownLeft)
                        {
                            ballDirection = ballDirectionDownRight;
                        }
                        else if (ballDirection == ballDirectionDownRight)
                        {
                            ballDirection = ballDirectionDownLeft;
                        }
                        else if (ballDirection == ballDirectionUpLeft)
                        {
                            ballDirection = ballDirectionUpRight;
                        }
                        else if (ballDirection == ballDirectionUpRight)
                        {
                            ballDirection = ballDirectionUpLeft;
                        }
                    }

                    // Collide down
                    if (bally > fieldHeight)
                    {
                        turns--;
                        if (turns <= 0)
                        {
                            gameState = gameStateEnd;
                            gameOverText = "Game Over";
                        }
                        bally = startingBallY;
                        ballDirection = startingBallDirection;
                    }

                    ballMoveCounter = 0;
                }
            }
            else if (gameState == gameStateEnd)
            {
                if (Engine.GetKeyDown(ConsoleKey.Enter))
                {
                    gameState = gameStateStart;
                }
            }

        }

        private bool collidePaddle()
        {
            bool result = false;

            if (bally >= paddley && ballx >= paddlex && ballx < (paddlex + width) && bally <= (paddley + height))
            {
                result = true;
            }

            return result;
        }
        private bool collideBlocks()
        {
            bool result = false;
            for (int y = 0; y < blockRows; y++)
            {
                for (int x = 0; x < blockCols; x++)
                {
                    if (blocks[y, x].alive)
                    {
                        if (bally >= blocks[y, x].y && bally <= (blocks[y, x].y + height) && ballx >= blocks[y, x].x && ballx < (blocks[y, x].x + width))
                        {
                            blocks[y, x].alive = false;
                            score += 10;
                            result = true;
                            if (allBlocksDead())
                            {
                                gameState = gameStateEnd;
                                gameOverText = "You Win!";
                            }
                            break;
                        }
                    }
                }
                if (result)
                {
                    break;
                }
            }
            return result;
        }

        private bool allBlocksDead()
        {
            bool result = true;
            for (int y = 0; y < blockRows; y++)
            {
                for (int x = 0; x < blockCols; x++)
                {
                    if (blocks[y, x].alive)
                    {
                        result = false;
                        break;
                    }
                }
                if (!result)
                {
                    break;
                }
            }
            return result;
        }

        private void reset()
        {
            paddlex = startingX;
            paddley = startingY;
            ballx = startingBallX;
            bally = startingBallY;
            ballWidth = defaultBallWidth;
            ballHeight = defaultBallHeight;
            ballVelx = startingBallXVel;
            ballVely = startingBallYVel;
            ballMoveCounter = 0;
            ballMoveDelay = startingBallMoveDelay;
            ballDirection = startingBallDirection;
            gameState = startingGameState;
            score = 0;
            turns = startingTurns;

            for (int y = 0; y < blockRows; y++)
            {
                for (int x = 0; x < blockCols; x++)
                {
                    blocks[y, x].alive = true;
                }
            }
        }
    }
}
