using System;
using System.Linq;

namespace _2048.Core
{
	/// <summary>
	/// Execute strategies on an empty board. Accumulates statistics over many runs for evaluation.
	/// </summary>
	internal static class Runner
	{
		private static Board TestStrategyOnce(IStrategy strategy)
		{
			(var running, var board) = Board.Empty.TrySpawn();
			while (running)
			{
#if DEBUG
				Console.WriteLine(board);
#endif
				if (!board.ValidShifts.Any()) break;
				board = board.Shift(strategy.GetMove(board));
				(running, board) = board.TrySpawn();
			}
			return board;
		}

		public static Score TestStrategy(IStrategy strategy, int runs = 2048)
		{
			var results = Enumerable.Range(0, runs).Select(_ => TestStrategyOnce(strategy)).ToArray();
			var scores = results.Select(b => b.Score).ToArray();
			var maxTiles = results.Select(b => b.Fields.Max()).ToArray();
			return new Score(scores.Max(), scores.Average(), scores.Min(), maxTiles.Max(), maxTiles.Average(), maxTiles.Min());
		}

		internal struct Score
		{
			public Score(int maxScore, double averageScore, int minScore, int maxMaxTile, double averageMaxTile, int minMaxTile)
			{
				MaxScore = maxScore;
				AverageScore = averageScore;
				MinScore = minScore;
				MaxMaxTile = maxMaxTile;
				MinMaxTile = minMaxTile;
				AverageMaxTile = averageMaxTile;
			}

			public int MaxScore { get; }

			public double AverageScore { get; }

			public int MinScore { get; }

			public int MaxMaxTile { get; }

			public double AverageMaxTile { get; }

			public int MinMaxTile { get; }
		}
	}
}
