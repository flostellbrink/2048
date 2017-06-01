using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace _2048
{
	class Program
	{
		static readonly Dictionary<char, Direction> Keymap = new Dictionary<char, Direction>
		{
			{'w', Direction.Up},
			{'s', Direction.Down},
			{'a', Direction.Left},
			{'d', Direction.Right}
		};

		static void Main(string[] args)
		{
			(var running, var board) = Board.Empty.TrySpawn();
			while (running)
			{
				Console.WriteLine(board);
				if(Keymap.TryGetValue(Console.ReadKey().KeyChar, out var direction))
					board = board.Shift(direction);
				else break;
				(running, board) = board.TrySpawn();
			}

			Console.WriteLine("Game Over");
			Console.ReadLine();
		}
	}

	internal enum Direction { Up, Down, Left, Right }

	/* 0 1 2 3
	 * 4 5 6 7
	 * 8 9 A B
	 * C D E F */
	internal class Board
	{
		public const int FieldSize = 4;

		private static readonly int[][][] Moveset = {
			new []{ new[]{ 0x0, 0x4, 0x8, 0xC }, new[] { 0x1, 0x5, 0x9, 0xD }, new[] { 0x2, 0x6, 0xA, 0xE }, new[] { 0x3, 0x7, 0xB, 0xF } },
			new []{ new[]{ 0xC, 0x8, 0x4, 0x0 }, new[] { 0xD, 0x9, 0x5, 0x1 }, new[] { 0xE, 0xA, 0x6, 0x2 }, new[] { 0xF, 0xB, 0x7, 0x3 } },
			new []{ new[]{ 0x0, 0x1, 0x2, 0x3 }, new[] { 0x4, 0x5, 0x6, 0x7 }, new[] { 0x8, 0x9, 0xA, 0xB }, new[] { 0xC, 0xD, 0xE, 0xF } },
			new []{ new[]{ 0x3, 0x2, 0x1, 0x0 }, new[] { 0x7, 0x6, 0x5, 0x4 }, new[] { 0xB, 0xA, 0x9, 0x8 }, new[] { 0xF, 0xE, 0xD, 0xC } }
		};

		private static readonly Random Random = new Random();

		public int[] Fields { get; set; }

		public static Board Empty => new Board {Fields = new int[FieldSize * FieldSize]};

		public Board Shift(Direction direction)
		{
			var fields = (int[])Fields.Clone();
			foreach (var move in Moveset[(int) direction])
				ApplyMove(ref fields, move);
			Debug.Assert(!fields.SequenceEqual(Fields), "Invalid move");
			return new Board { Fields = fields };
		}

		public (bool, Board) TryShift(Direction direction)
		{
			var fields = (int[])Fields.Clone();
			foreach (var move in Moveset[(int)direction])
				ApplyMove(ref fields, move);
			return (fields.SequenceEqual(Fields), new Board { Fields = fields });
		}

		private static void ApplyMove(ref int[] fields, IReadOnlyList<int> move)
		{
			// Iterate over potentially moving fields (starting from second)
			for (var i = 1; i < FieldSize; i++)
			{
				// Value 0 cannot move
				if (fields[move[i]] == 0) continue;

				// Find closest non 0 field in direction of move
				var j = i;
				while (--j > 0 && fields[move[j]] == 0) { }

				// Reached last field which is not zero -> Move field there
				if (fields[move[j]] == 0)
				{
					fields[move[j]] = fields[move[i]];
					fields[move[i]] = 0;
				}
				// Found same value as field -> Merge them
				else if (fields[move[i]] == fields[move[j]])
				{
					fields[move[i]] = 0;
					fields[move[j]] <<= 1;
				}
				// Found other value, check if last non zero field is different from current -> Move field there
				else if (i != j + 1)
				{
					fields[move[j + 1]] = fields[move[i]];
					fields[move[i]] = 0;
				}
			}
		}

		public (bool, Board) TrySpawn()
		{
			var available = Fields.Select((v, i) => new {v, i}).Where(f => f.v == 0).ToArray();
			if(!available.Any()) return (false, this);
			var field = available[Random.Next(available.Length)];
			var value = Random.Next(9) == 9 ? 4 : 2;
			return (true, Spawn(field.i, value));
		}

		public Board Spawn(int index, int value)
		{
			Debug.Assert(index >= 0 && index <= 0xF);
			Debug.Assert(Fields[index] == 0);
			var fields = (int[])Fields.Clone();
			fields[index] = value;
			return new Board { Fields = fields };
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			var counter = 0;
			for (var i = 0; i < FieldSize; i++)
			{
				builder.AppendLine();
				for (var j = 0; j < FieldSize; j++)
				{
					builder.Append($"{Fields[counter++],4}");
				}
				builder.AppendLine();
			}
			return builder.ToString();
		}
	}
}
