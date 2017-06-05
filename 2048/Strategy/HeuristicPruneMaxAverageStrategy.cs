using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoreLinq;
using _2048.Core;
using _2048.ScoreFunction;

namespace _2048.Strategy
{
	/// <summary>
	/// Since we are not using Minimax Alpha/Beta Pruning can not be applied.
	/// Evaluate scores of shifts directly. Prune all but the top <see cref="_evaluateShiftsCount"/> paths.
	/// Ignore spawn of less than average probability.
	/// </summary>
	internal class HeuristicPruneMaxAverageStrategy : DepthFirstStrategy
	{
		private readonly IEvaluator _evaluator;

		private readonly int _evaluateShiftsCount;

		public HeuristicPruneMaxAverageStrategy(IEvaluator evaluator, TimeSpan limit, int evaluateShiftsCount) : base(limit)
		{
			_evaluator = evaluator;
			_evaluateShiftsCount = evaluateShiftsCount;
		}

		protected override Direction GetMove(Board board, int depth)
		{
			Debug.Assert(board.ValidShifts.Any(), "Invalid board state");
			return board.ValidShifts.MaxBy(s => AverageSpawnValue(s.Value, depth - 1)).Key;
		}

		private double BestShiftValue(Board board, int remainingDepth)
		{
			if (!board.ValidShifts.Any())
				return 0;
			if (remainingDepth <= 0)
				return board.ValidShifts.Select(s => _evaluator.GetScore(s.Value)).Max();
			return board.ValidShifts
				.OrderBy(s => _evaluator.GetScore(s.Value), OrderByDirection.Descending)
				.Take(_evaluateShiftsCount)
				.Select(s => AverageSpawnValue(s.Value, remainingDepth - 1)).Max();
		}

		private double AverageSpawnValue(Board board, int remainingDepth)
		{
			if (!board.ValidSpawns.Any())
				return 0;
			if (remainingDepth <= 0)
				return board.ValidSpawns.Sum(s => s.Item1 * _evaluator.GetScore(s.Item2));
			var averageProbability = 1.0 / board.ValidSpawns.Count;
			return board.ValidSpawns
				.Where(s => s.Item1 > averageProbability)
				.Sum(s => s.Item1 * BestShiftValue(s.Item2, remainingDepth - 1));
		}
	}
}
