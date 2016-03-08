using System;

enum MenuItem : byte
{
	Start,
	Help,
	Exit,
	Resume,
	BackToMain,
	Pause
}

static class Menu
{
	static MenuItem activeMenu = MenuItem.Start;	// Set active menu item on fist run here

	public static MenuItem Active
	{
		get
		{
			return activeMenu;
		}
		set
		{
			activeMenu = value;
		}
	}

	public static void Up()
	{
		switch (activeMenu)
		{
			case MenuItem.Start:
				break;
			case MenuItem.Help:
				activeMenu = MenuItem.Start;
				break;
			case MenuItem.Exit:
				activeMenu = MenuItem.Help;
				break;
			case MenuItem.Resume:
				break;
			case MenuItem.BackToMain:
				break;
			case MenuItem.Pause:
				break;
		}
	}

	public static void Down()
	{
		switch (activeMenu)
		{
			case MenuItem.Start:
				activeMenu = MenuItem.Help;
				break;
			case MenuItem.Help:
				activeMenu = MenuItem.Exit;
				break;
			case MenuItem.Exit:
				break;
			case MenuItem.Resume:
				break;
			case MenuItem.BackToMain:
				break;
			case MenuItem.Pause:
				break;
		}
	}

	public static void Activate()
	{
		switch (activeMenu)
		{
			case MenuItem.Start:
				Tetris.gameState = BoardState.Active;
				break;
			case MenuItem.Help:
				Tetris.gameState = BoardState.Help;
				break;
			case MenuItem.Exit:
				Tetris.gameState = BoardState.Quit;
				break;
			case MenuItem.Resume:
				break;
			case MenuItem.BackToMain:
				break;
			case MenuItem.Pause:
				break;
		}
	}

	public static void Help()
	{
		// Clear the screen
		Console.BackgroundColor = ConsoleColor.Black;
		Console.Clear();

		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.SetCursorPosition(1, 1);
		Console.Write("CONTROLS:");

		Console.ForegroundColor = ConsoleColor.White;
		Console.SetCursorPosition(1, 2);
		Console.Write("Arrow keys - Move block");
		Console.SetCursorPosition(1, 3);
		Console.Write("Up Arrow   - Land block");
		Console.SetCursorPosition(1, 4);
		Console.Write("Space      - Rotate block");
		Console.SetCursorPosition(1, 5);
		Console.Write("P          - Pause game");
		Console.SetCursorPosition(1, 6);
		Console.Write("Escape     - Return to menu");

		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.SetCursorPosition(1, 8);
		Console.Write("OBJECTIVE:");
		Console.ForegroundColor = ConsoleColor.White;
		Console.SetCursorPosition(1, 9);
		Console.Write("Line up blocks before they land");
		Console.SetCursorPosition(1, 10);
		Console.Write("and try and complete a line with");
		Console.SetCursorPosition(1, 11);
		Console.Write("no gaps to remove it and score");
		Console.SetCursorPosition(1, 12);
		Console.Write("points. Complete more than one");
		Console.SetCursorPosition(1, 13);
		Console.Write("line at a time for a bonus. Game");
		Console.SetCursorPosition(1, 14);
		Console.Write("speed will increase over time,");
		Console.SetCursorPosition(1, 15);
		Console.Write("try to think fast!");

		Console.BackgroundColor = ConsoleColor.Green;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.SetCursorPosition(1, 17);
		Console.Write(" BACK        ");

		while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
		Console.Beep(200, 100);
		Tetris.gameState = BoardState.MainMenu;
	}

	public static void Draw()
	{
		// Clear the screen
		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Clear();

		// Print menu buttons, highlight active item
		Console.SetCursorPosition(1, 2);
		if (activeMenu == MenuItem.Start) Console.BackgroundColor = ConsoleColor.Green;
		else Console.BackgroundColor = ConsoleColor.DarkGreen;
		Console.Write(" START       ");

		Console.SetCursorPosition(1, 4);
		if (activeMenu == MenuItem.Help) Console.BackgroundColor = ConsoleColor.Green;
		else Console.BackgroundColor = ConsoleColor.DarkGreen;
		Console.Write(" HOW TO PLAY ");

		Console.SetCursorPosition(1, 6);
		if (activeMenu == MenuItem.Exit) Console.BackgroundColor = ConsoleColor.Green;
		else Console.BackgroundColor = ConsoleColor.DarkGreen;
		Console.Write(" QUIT        ");

		// Copyright information
		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.SetCursorPosition(1, 13);
		Console.Write("Tetris \u00A9 Alexey Pajitnov");
		Console.SetCursorPosition(1, 15);
		Console.Write("Educational implementation");
		Console.SetCursorPosition(1, 16);
		Console.Write("by Viktor Kostadinov. Free to");
		Console.SetCursorPosition(1, 17);
		Console.Write("modify/distribute with original");
		Console.SetCursorPosition(1, 18);
		Console.Write("credit to Alexey Pajitnov.");
	}
}