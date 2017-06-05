using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace _2048.Core
{
	/* 0 1 2 3
	 * 4 5 6 7
	 * 8 9 A B
	 * C D E F */
	internal class Board
	{
		private Board(int[] fields, int score = 0)
		{
			Fields = fields;
			Score = score;
		}

		public const int FieldSize = 4;

		private static readonly int[][][] MoveSet = {
			new []{ new[]{ 0x0, 0x4, 0x8, 0xC }, new[] { 0x1, 0x5, 0x9, 0xD }, new[] { 0x2, 0x6, 0xA, 0xE }, new[] { 0x3, 0x7, 0xB, 0xF } },
			new []{ new[]{ 0xC, 0x8, 0x4, 0x0 }, new[] { 0xD, 0x9, 0x5, 0x1 }, new[] { 0xE, 0xA, 0x6, 0x2 }, new[] { 0xF, 0xB, 0x7, 0x3 } },
			new []{ new[]{ 0x0, 0x1, 0x2, 0x3 }, new[] { 0x4, 0x5, 0x6, 0x7 }, new[] { 0x8, 0x9, 0xA, 0xB }, new[] { 0xC, 0xD, 0xE, 0xF } },
			new []{ new[]{ 0x3, 0x2, 0x1, 0x0 }, new[] { 0x7, 0x6, 0x5, 0x4 }, new[] { 0xB, 0xA, 0x9, 0x8 }, new[] { 0xF, 0xE, 0xD, 0xC } }
		};

		private static readonly Random Random = new Random();

		public int[] Fields { get; }

		public int Score { get; }

		public static Board Empty => new Board(new int[FieldSize * FieldSize]);

		/// <summary>
		/// Shift the board in <paramref name="direction"/>. Assumes that the move is valid
		/// </summary>
		/// <param name="direction">Direction to shift board into</param>
		/// <returns>Board state after moving</returns>
		public Board Shift(Direction direction, bool final = false)
		{
			Debug.Assert(ValidShifts.ContainsKey(direction), "Invalid move");
			var result = ValidShifts[direction];
			if (final) _validShifts = null;
			return result;
		}

		private IDictionary<Direction, Board> _validShifts;

		public IDictionary<Direction, Board> ValidShifts => _validShifts ?? (_validShifts = GetValidShifts());

		/// <summary>
		/// Enumerate all valid moves
		/// </summary>
		/// <returns>Possible shift directions and resulting boards</returns>
		private Dictionary<Direction, Board> GetValidShifts()
		{
			var result = new Dictionary<Direction, Board>();
			foreach (var direction in EnumHelper.GetValues<Direction>())
			{
				var fields = (int[])Fields.Clone();
				var score = Score;
				foreach (var move in MoveSet[(int)direction])
					Shift(ref fields, move, ref score);
				if(!fields.SequenceEqual(Fields))
					result.Add(direction, new Board(fields, score));
			}
			return result;
		}

		/// <summary>
		/// Apply a move to the <paramref name="fields"/>
		/// </summary>
		/// <param name="fields">Fields to manipulate</param>
		/// <param name="move">Column of fields to move</param>
		/// <param name="score">Score to increase in case of merge</param>
		private static void Shift(ref int[] fields, IReadOnlyList<int> move, ref int score)
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
					score += fields[move[j]];
				}
				// Found other value, check if last non zero field is different from current -> Move field there
				else if (i != j + 1)
				{
					fields[move[j + 1]] = fields[move[i]];
					fields[move[i]] = 0;
				}
			}
		}

		private IList<(double, Board)> _validSpawns;

		public IList<(double, Board)> ValidSpawns => _validSpawns ?? (_validSpawns = GetValidSpawns());

		/// <summary>
		/// Enumerate valid spawns
		/// </summary>
		/// <returns>List of probability and board states after each spawn</returns>
		private IList<(double, Board)> GetValidSpawns()
		{
			var available = Fields.Select((v, i) => new { v, i }).Where(f => f.v == 0).ToArray();
			double prob2 = 1.0 / available.Length * .9, prob4 = 1.0 / available.Length * .1;
			return available.Select(f => (prob2, Spawn(f.i, 2)))
				.Concat(available.Select(f => (prob4, Spawn(f.i, 4))))
				.ToArray();

			Board Spawn(int index, int value)
			{
				Debug.Assert(index >= 0 && index <= 0xF);
				Debug.Assert(Fields[index] == 0);
				var fields = (int[])Fields.Clone();
				fields[index] = value;
				return new Board(fields, Score);
			}
		}

		/// <summary>
		/// Attempt to spawn a random tile on the board
		/// </summary>
		/// <returns>Tuple. First value indicates if spawning is possible, second value the board state after spawning</returns>
		public Board Spawn(bool final = false)
		{
			var result = ValidSpawns[Random.Next(ValidSpawns.Count)].Item2;
			if (final) _validSpawns = null;
			return result;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			var counter = 0;
			builder.AppendLine();
			builder.Append("Score: ").AppendLine(Score.ToString());
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