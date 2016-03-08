using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

class Tetris
{
	static void ProcessInput()
	{
		if (!Console.KeyAvailable) { return; }
		ConsoleKeyInfo key = Console.ReadKey(true);
		switch (key.Key)
		{
			// TODO: Rest of the controls
			case ConsoleKey.LeftArrow:
				// Move left
				Tetris.activeBlock.MoveLeft();
				break;
			case ConsoleKey.RightArrow:
				// Move right
				Tetris.activeBlock.MoveRight();
				break;
			case ConsoleKey.DownArrow:
				// Fall
				Tetris.activeBlock.Fall();
				break;
			case ConsoleKey.UpArrow:
				// Drop
				Tetris.activeBlock.Drop();
				break;
			case ConsoleKey.Spacebar:
				// Rotate
				Tetris.activeBlock.RotateLeft();
				break;
			case ConsoleKey.Escape:
				// DEBUG
				Quit();
				break;
			case ConsoleKey.P:
				Pause();
				break;
		}
	}

	static void Pause()
	{
		Console.Beep(200, 100);
		for (int y = 0; y < 20; y++)
		{
			for (int x = 0; x < 10; x++)
			{
				Tetris.screenBuffer[x, y] = '0';
			}
		}
		Board.Draw(screenBuffer);
		Console.SetCursorPosition(7, 10);
		Console.Write("PAUSED");

		ConsoleKeyInfo key;
		do key = Console.ReadKey(true);
		while (key.Key != ConsoleKey.P && key.Key != ConsoleKey.Escape);
		Console.Beep(200, 100);
	}

	static void ScoreScreen()
	{
		Console.BackgroundColor = ConsoleColor.DarkRed;
		Console.ForegroundColor = ConsoleColor.White;
		Console.SetCursorPosition(4, 9);
		Console.Write("           ");
		Console.SetCursorPosition(4, 10);
		Console.Write(" GAME OVER ");
		Console.SetCursorPosition(4, 11);
		Console.Write("           ");
		Console.Beep(2000, 50);
		Console.Beep(500, 950);
		Console.ReadKey(true);
		Tetris.gameState = BoardState.MainMenu;
		Console.Beep(300, 100);
	}

	static void Quit()
	{
		Console.Beep(200, 100);
		for (int y = 0; y < 20; y++)
		{
			for (int x = 0; x < 10; x++)
			{
				Tetris.screenBuffer[x, y] = '0';
			}
		}
		Board.Draw(screenBuffer);
		Console.SetCursorPosition(1, 10);
		Console.Write("CONFIRM QUIT? (Y/N)");
		ConsoleKeyInfo key = Console.ReadKey(true);
		Console.Beep(200, 100);
		if (key.Key == ConsoleKey.Y)
		{
			gameState = BoardState.MainMenu;
		}
		else
			return;
	}

	static void ProcessBoard(double elapsed)
	{
		// Wipe the buffer and add existing blocks
		for (int y = 0; y < 20; y++)
		{
			for (int x = 0; x < 10; x++)
			{
				if ((board[y + 4] & (1 << x + 2)) != 0)
				{
					Tetris.screenBuffer[x, y] = '1';
				}
				else
				{
					Tetris.screenBuffer[x, y] = '0';
				}
			}
		}
		// Time to fall
		if (elapsed > lastUpdate + (200 + (16 - level) * 50))
		{
			activeBlock.Fall();
			lastUpdate = elapsed;
		}
		// Time to increase level (every 40 seconds)
		if (level < 16 && elapsed > lastLevel + 40000)
		{
			level++;
			lastLevel = elapsed;
		}
		// Transfer active block
		activeBlock.ToBuffer();
	}

	static void MenuLoop()
	{
		// Draw the menu
		Menu.Draw();
		// Process input
		ConsoleKeyInfo key = Console.ReadKey(true);
		switch (key.Key)
		{
			case ConsoleKey.UpArrow:
				Menu.Up();
				break;
			case ConsoleKey.DownArrow:
				Menu.Down();
				break;
			case ConsoleKey.Enter:
				Console.Beep(200, 100);
				Menu.Activate();
				break;
		}
	}

	static void GameLoop()
	{
		board = new ushort[27];
		lastUpdate = 0;
		lastLevel = 0;
		level = 1;
		score = 0;
		alive = true;

		Board.Initialize();
		activeBlock = new ActiveBlock();
		activeBlock.Spawn();
		// Start timer
		Stopwatch timer = new Stopwatch();
		timer.Start();

		// Game loop
		while (gameState == BoardState.Active)
		{
			// Process events
			ProcessInput();
			ProcessBoard(timer.Elapsed.TotalMilliseconds);
			// Draw board
			Board.Draw(screenBuffer);
			if (!alive)
			{
				gameState = BoardState.ScoreScreen;
				break;
			}
		}
	}

	// Global variables
	public static BoardState gameState = BoardState.MainMenu;
	public static bool alive = false;
	static double lastUpdate;   // Last time block was auto-advanced
	static double lastLevel;    // Last time diffculty was increased
	public static int level;
	public static int score;
	public static ushort[] board;
	public static char[,] screenBuffer = new char[10, 20];
	public static ActiveBlock activeBlock;

	static void Main()
	{
		Console.OutputEncoding = Encoding.UTF8;

		// Setup playable area
		Console.Title = "Tetris";
		Console.CursorVisible = false;
		Console.WindowHeight = 20;
		Console.BufferHeight = 20;
		Console.WindowWidth = 34;
		Console.BufferWidth = 34;


		// Game loop, first run lands in menu
		while (true)
		{
			switch (gameState)
			{
				case BoardState.MainMenu:
					MenuLoop();
					break;
				case BoardState.Help:
					Menu.Help();
					break;
				case BoardState.Active:
					GameLoop();
					break;
				case BoardState.Paused:
					break;
				case BoardState.ScoreScreen:
					ScoreScreen();
					break;
				case BoardState.Quit:
					return;
			}
		}
	}
}