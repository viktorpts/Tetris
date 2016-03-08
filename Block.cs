using System;

class Block
{
	byte type = 0;
	// 0 - null
	// 1 - box
	// 2 - line
	// 3 - T
	// 4 - L
	// 5 - L reverse
	// 6 - S
	// 7 - S reverse
	byte direction = 0;
	// 0 - East
	// 1 - North
	// 2 - West (may not exist)
	// 3 - South (may not exist)

	public byte Type { get { return type; } }

	public Block()
	{
		// Create random block
		type = (byte)(new Random()).Next(1, 8);
		direction = 0;
	}
	public void RotateLeft()
	{
		switch (type)
		{
			case 1:
				break;
			case 2:
			case 6:
			case 7:
				if (direction == 0) { direction = 1; }
				else { direction = 0; }
				break;
			case 3:
			case 4:
			case 5:
				if (direction == 0) { direction = 3; }
				else { direction--; }
				break;
		}
	}
	public void RotateRight()
	{
		switch (type)
		{
			case 1:
				break;
			case 2:
			case 6:
			case 7:
				if (direction == 0) { direction = 1; }
				else { direction = 0; }
				break;
			case 3:
			case 4:
			case 5:
				if (direction == 3) { direction = 0; }
				else { direction++; }
				break;
		}
	}
	public ushort Render()
	{
		// Render object and return it's representation based on type and direction
		// Bit index reference matrix:
		// C D E F
		// 8 9 A B
		// 4 5 6 7
		// 0 1 2 3
		switch (type)
		{
			case 1:
				return 51;
			case 2:
				if (direction == 0) { return 8738; }
				else { return 3840; }
			case 3:
				if (direction == 0) { return 114; }
				else if (direction == 1) { return 305; }
				else if (direction == 2) { return 39; }
				else { return 562; }
			case 4:
				if (direction == 0) { return 113; }
				else if (direction == 1) { return 275; }
				else if (direction == 2) { return 71; }
				else { return 802; }
			case 5:
				if (direction == 0) { return 116; }
				else if (direction == 1) { return 785; }
				else if (direction == 2) { return 23; }
				else { return 547; }
			case 6:
				if (direction == 0) { return 99; }
				else { return 306; }
			case 7:
				if (direction == 0) { return 54; }
				else { return 561; }
			default:
				return 0;
		}
	}
}

class ActiveBlock
{
	int x, y;
	Block current;
	static Block next;
	static uint count = 0;

	public ActiveBlock()
	{
		// Initialize next block to a random block
		next = new Block();
	}

	public ushort Next { get { return next.Render(); } }

	// Controls
	public void MoveLeft()
	{
		if (Board.Validate(Tetris.board, current, x - 1, y) == BlockState.Valid)
		{
			x--;
		}
	}
	public void MoveRight()
	{
		if (Board.Validate(Tetris.board, current, x + 1, y) == BlockState.Valid)
		{
			x++;
		}
	}
	public void RotateLeft()
	{
		Block target = current;
		target.RotateLeft();
		BlockState result = Board.Validate(Tetris.board, target, x, y);
		if (result == BlockState.Valid) { current = target; }
		else if (Reposition(target, result)) { current = target; }
	}
	public void RotateRight()
	{
		Block target = current;
		target.RotateRight();
		BlockState result = Board.Validate(Tetris.board, target, x, y);
		if (result == BlockState.Valid) { current = target; }
		else if (Reposition(target, result)) { current = target; }
	}
	public bool Reposition(Block target, BlockState result)
	{
		// Collision is on top
		if ((result & BlockState.Top) != 0)
		{
			// TODO: Attemp to shift block down
			return false;
		}
		// Collision on the left, right is open
		if ((result & BlockState.Hit1) != 0 && (result & BlockState.Hit4) == 0)
		{
			// Double collision
			if ((result & BlockState.Hit2) != 0 && Board.Validate(Tetris.board, target, x + 2, y) == BlockState.Valid)
			{
				// New position found, relocate
				x += 2;
				return true;
			}
			else if (Board.Validate(Tetris.board, target, x + 1, y) == BlockState.Valid)
			{
				// New position found, relocate
				x += 1;
				return true;
			}
			else
			{
				// New position is invalid, cancel
				return false;
			}
		}
		// Collision on the right, left is open
		else if ((result & BlockState.Hit4) != 0 && (result & BlockState.Hit1) == 0)
		{
			// Double collision
			if ((result & BlockState.Hit3) != 0 && Board.Validate(Tetris.board, target, x - 2, y) == BlockState.Valid)
			{
				// New position found, relocate
				x -= 2;
				return true;
			}
			else if (Board.Validate(Tetris.board, target, x - 1, y) == BlockState.Valid)
			{
				// New position found, relocate
				x -= 1;
				return true;
			}
			else
			{
				// New position is invalid, cancel
				return false;
			}
		}
		else
		{
			// Block is confined on both sides
			return false;
		}
	}
	public void Fall()
	{
		BlockState result = Board.Validate(Tetris.board, current, x, y + 1);
		if (result == BlockState.Valid)
		{
			y++;
		}
		else
		{
			Console.Beep(150, 100);
			Tetris.board = Board.Commit(Tetris.board, current, x, y);
			Tetris.board = Board.Lines(Tetris.board);
			Spawn();
		}
	}
	public void Drop()
	{
		while (Board.Validate(Tetris.board, current, x, y + 1) == BlockState.Valid)
		{
			Fall();
		}
	}
	public void Spawn()
	{
		// Cycle next block
		current = next;
		next = new Block();
		// Move to screen top center
		x = 4;
		if (current.Type == 2)
		{
			y = 3;
		}
		else
		{
			y = 1;
		}
		// Increase count
		count++;
		// Make there is enough room
		if (Board.Validate(Tetris.board, current, x, y) != BlockState.Valid)
		{
			Tetris.alive = false;
		}
	}
	public void ToBuffer()
	{
		// Output representaion to buffer
		ushort sprite = current.Render();
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int index = j + i * 4;
				if ((sprite & (1 << index)) != 0 && (y - i) >= 0) { Tetris.screenBuffer[x + j, y - i] = '2'; }
			}
		}
	}
}