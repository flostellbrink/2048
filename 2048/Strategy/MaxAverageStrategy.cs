using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MoreLinq;
using _2048.Core;
using _2048.ScoreFunction;

namespace _2048.Strategy
{
	/// <summary>
	/// Look at average for random steps, maximize non random, don't purge
	/// </summary>
	internal class MaxAverageStrategy : DepthFirstStrategy
	{
		private readonly IEvaluator _evaluator;

		public MaxAverageStrategy(IEvaluator evaluator, TimeSpan limit) : base(limit)
		{
			_evaluator = evaluator;
		}

		protected override Direction GetMove(Board board, int depth, CancellationToken token)
		{
			Debug.Assert(board.ValidShifts.Any(), "Invalid board state");
			return board.ValidShifts.MaxBy(s => AverageSpawnValue(s.Value, depth - 1, token)).Key;
		}

		private double BestShiftValue(Board board, int remainingDepth, CancellationToken token)
		{
			if (token.IsCancellationRequested)
				return 0;
			if (!board.ValidShifts.Any())
				return 0;
			return remainingDepth > 0 
				? board.ValidShifts.Select(s => AverageSpawnValue(s.Value, remainingDepth - 1, token)).Max() 
				: board.ValidShifts.Select(s => _evaluator.GetScore(s.Value)).Max();
		}

		private double AverageSpawnValue(Board board, int remainingDepth, CancellationToken token)
		{
			if (token.IsCancellationRequested)
				return 0;
			if (!board.ValidSpawns.Any())
				return 0;
			return remainingDepth > 0 
				? board.ValidSpawns.Sum(s => s.Item1 * BestShiftValue(s.Item2, remainingDepth - 1, token)) 
				: board.ValidSpawns.Sum(s => s.Item1 * _evaluator.GetScore(s.Item2));
		}
	}
}
