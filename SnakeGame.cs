using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static int width = 40;
        static int height = 20;
        static int score = 0;
        static int highScore = 0;
        static bool gameOver = false;
        static Direction direction = Direction.Right;
        static Direction nextDirection = Direction.Right;
        static List<Point> snake = new List<Point>();
        static Point food;
        static Random random = new Random();
        static readonly int gameSpeed = 100; // milliseconds

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(width + 2, height + 6);
            Console.SetBufferSize(width + 2, height + 6);

            while (true)
            {
                InitializeGame();
                GameLoop();
                ShowGameOver();
                
                Console.SetCursorPosition(0, height + 4);
                Console.Write("Press R to restart or Q to quit... ");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q) break;
            }
        }

        static void InitializeGame()
        {
            snake.Clear();
            int startX = width / 2;
            int startY = height / 2;
            
            // Initial snake body (3 segments)
            snake.Add(new Point(startX, startY));
            snake.Add(new Point(startX - 1, startY));
            snake.Add(new Point(startX - 2, startY));
            
            direction = Direction.Right;
            nextDirection = Direction.Right;
            score = 0;
            gameOver = false;
            
            SpawnFood();
            DrawBorder();
            DrawScore();
        }

        static void GameLoop()
        {
            while (!gameOver)
            {
                HandleInput();
                Update();
                Draw();
                Thread.Sleep(gameSpeed);
            }
        }

        static void HandleInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (direction != Direction.Down) nextDirection = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (direction != Direction.Up) nextDirection = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        if (direction != Direction.Right) nextDirection = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        if (direction != Direction.Left) nextDirection = Direction.Right;
                        break;
                    case ConsoleKey.Escape:
                        gameOver = true;
                        break;
                }
            }
        }

        static void Update()
        {
            direction = nextDirection;
            
            Point head = snake[0];
            Point newHead = new Point(head.X, head.Y);
            
            switch (direction)
            {
                case Direction.Up: newHead.Y--; break;
                case Direction.Down: newHead.Y++; break;
                case Direction.Left: newHead.X--; break;
                case Direction.Right: newHead.X++; break;
            }

            // Check wall collision
            if (newHead.X <= 0 || newHead.X >= width - 1 || newHead.Y <= 0 || newHead.Y >= height - 1)
            {
                gameOver = true;
                return;
            }

            // Check self collision
            foreach (Point segment in snake)
            {
                if (segment.X == newHead.X && segment.Y == newHead.Y)
                {
                    gameOver = true;
                    return;
                }
            }

            snake.Insert(0, newHead);

            // Check food collision
            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score += 10;
                if (score > highScore) highScore = score;
                DrawScore();
                SpawnFood();
            }
            else
            {
                // Remove tail if no food eaten
                Point tail = snake[snake.Count - 1];
                Console.SetCursorPosition(tail.X, tail.Y);
                Console.Write(' ');
                snake.RemoveAt(snake.Count - 1);
            }
        }

        static void Draw()
        {
            // Draw new head
            Point head = snake[0];
            Console.SetCursorPosition(head.X, head.Y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('█');

            // Draw body
            for (int i = 1; i < snake.Count; i++)
            {
                Console.SetCursorPosition(snake[i].X, snake[i].Y);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write('█');
            }

            // Draw food
            Console.SetCursorPosition(food.X, food.Y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write('●');
            
            Console.ResetColor();
        }

        static void SpawnFood()
        {
            bool validPosition = false;
            while (!validPosition)
            {
                food = new Point(random.Next(1, width - 1), random.Next(1, height - 1));
                validPosition = true;
                
                foreach (Point segment in snake)
                {
                    if (segment.X == food.X && segment.Y == food.Y)
                    {
                        validPosition = false;
                        break;
                    }
                }
            }
        }

        static void DrawBorder()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            
            // Top and bottom borders
            for (int x = 0; x < width; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write('█');
                Console.SetCursorPosition(x, height - 1);
                Console.Write('█');
            }
            
            // Left and right borders
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write('█');
                Console.SetCursorPosition(width - 1, y);
                Console.Write('█');
            }
            
            Console.ResetColor();
        }

        static void DrawScore()
        {
            Console.SetCursorPosition(0, height + 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Score: {score}  High Score: {highScore}");
            Console.ResetColor();
            
            Console.SetCursorPosition(0, height + 2);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Controls: Arrow Keys / WASD | ESC to quit");
            Console.ResetColor();
        }

        static void ShowGameOver()
        {
            Console.SetCursorPosition(width / 2 - 5, height / 2 - 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("GAME OVER");
            
            Console.SetCursorPosition(width / 2 - 8, height / 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Final Score: {score}");
            
            Console.SetCursorPosition(width / 2 - 8, height / 2 + 1);
            Console.Write($"High Score: {highScore}");
            Console.ResetColor();
        }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}