using System;

enum BlockState : byte
{
	Hit1 = 0x01,
	Hit2 = 0x02,
	Hit3 = 0x04,
	Hit4 = 0x08,
	Valid = 0x00,
	Bottom = 0x10,
	Block = 0x20,
	Top = 0x40
}

enum BoardState : byte
{
	MainMenu,
	Help,
	Active,
	Paused,
	ScoreScreen,
	Quit
}

static class Board
{
	public static BlockState Validate(ushort[] board, Block target, int x, int y)
	{
		// Check for collisions and out of bounds errors
		ushort leftBounds2 = 1;
		ushort leftBounds1 = 2;
		ushort rightBounds4 = 61440;
		ushort rightBounds3 = 57344;
		for (int i = 0; i < 4; i++)
		{
			ushort mask = (ushort)(target.Render() >> (i * 4));
			mask &= 15;
			mask <<= 2 + x;
			// Bounds check
			if ((leftBounds1 & mask) != 0)
			{
				if ((leftBounds2 & mask) != 0)
				{
					return BlockState.Hit2 | BlockState.Hit1;
				}
				else
				{
					return BlockState.Hit1;
				}
			}
			if ((rightBounds4 & mask) != 0)
			{
				if ((rightBounds3 & mask) != 0)
				{
					return BlockState.Hit3 | BlockState.Hit4;
				}
				else
				{
					return BlockState.Hit4;
				}
			}
			// Bottom of board check
			if ((y - i > 19) && (mask & 65535) != 0)
			{
				return BlockState.Bottom;
			}
			// Top of board check
			if ((y - i < 0) && (mask & 65535) != 0)
			{
				return BlockState.Top;
			}
			// Blocks check
			if ((board[y + 4 - i] & mask) != 0)
			{
				return BlockState.Block;
			}
		}
		return BlockState.Valid;
	}

	public static ushort[] Commit(ushort[] board, Block target, int x, int y)
	{
		// Transfer block to static blocks
		// TODO
		ushort sprite = target.Render();
		for (int i = 0; i < 4; i++)
		{
			board[y + 4 - i] |= (ushort)(((sprite >> (i * 4)) & 15) << (2 + x));
		}
		return board;
	}

	public static ushort[] Lines(ushort[] board)
	{
		// Check for full lines
		// TODO
		ushort line = 4092;
		int multiplier = 0;
		for (int i = 19; i >= 0; i--)
		{
			if ((board[i + 4] ^ line) == 0)
			{
				for (int j = i; j > 0; j--)
				{
					board[j + 4] = board[j + 3];
				}
				board[4] = 0;
				multiplier++;
				i++;
				Tetris.score += Tetris.level * 10;
			}
		}
		Tetris.score += (multiplier * multiplier * 100);
		return board;
	}

	public static void Initialize()
	{
		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.White;
		Console.Clear();
		// Draw info
		Console.BackgroundColor = ConsoleColor.Black;
		Console.SetCursorPosition(21, 1);
		Console.Write("Next block");
		Console.SetCursorPosition(21, 9);
		Console.Write("Score {0}", Tetris.score);
		Console.SetCursorPosition(21, 10);
		Console.Write("Level {0}", Tetris.level);
		// Draw next block
		for (int i = 0; i < 6; i++)
		{
			Console.BackgroundColor = ConsoleColor.DarkGray;
			Console.SetCursorPosition(21, 2 + i);
			for (int j = 0; j < 6; j++)
			{
				Console.Write("  ");
			}
		}
	}

	public static void Draw(char[,] buffer)
	{
		Console.ForegroundColor = ConsoleColor.White;
		// TODO: Pretty colors, UI
		for (int y = 0; y < 20; y++)
		{
			Console.SetCursorPosition(0, y);
			for (int x = 0; x < 10; x++)
			{
				switch (buffer[x, y])
				{
					case '0':
						Console.BackgroundColor = ConsoleColor.DarkGray;
						break;
					case '1':
						Console.BackgroundColor = ConsoleColor.DarkGreen;
						break;
					case '2':
						Console.BackgroundColor = ConsoleColor.Green;
						break;
				}
				Console.Write("  ");
			}
		}
		// Draw info
		Console.BackgroundColor = ConsoleColor.Black;
		Console.SetCursorPosition(27, 9);
		Console.Write(Tetris.score);
		Console.SetCursorPosition(27, 10);
		Console.Write(Tetris.level);
		// Draw next block
		ushort sprite = Tetris.activeBlock.Next;
		for (int i = 0; i < 4; i++)
		{
			Console.BackgroundColor = ConsoleColor.DarkGray;
			Console.SetCursorPosition(23, 6 - i);
			for (int j = 0; j < 4; j++)
			{
				int index = j + i * 4;
				if ((sprite & (1 << index)) != 0)
				{
					Console.BackgroundColor = ConsoleColor.Green;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.DarkGray;
				}
				Console.Write("  ");
			}
		}
	}
}