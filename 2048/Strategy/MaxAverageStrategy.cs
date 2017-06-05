using System.Diagnostics;
using System.Linq;
using MoreLinq;
using _2048.Core;
using _2048.ScoreFunction;

namespace _2048.Strategy
{
	/// <summary>
	/// Look at average for random steps, maximize non random, don't purge
	/// </summary>
	internal class MaxAverageStrategy : IStrategy
	{
		private readonly IEvaluator _evaluator;

		private readonly int _depth;

		public MaxAverageStrategy(IEvaluator evaluator, int depth)
		{
			_evaluator = evaluator;
			_depth = depth;
		}

		public Direction GetMove(Board board)
		{
			Debug.Assert(board.ValidShifts.Any(), "Invalid board state");
			return board.ValidShifts.MaxBy(s => AverageSpawnValue(s.Value, _depth - 1)).Key;
		}

		private double BestShiftValue(Board board, int remainingDepth)
		{
			if (!board.ValidShifts.Any())
				return 0;
			return remainingDepth > 0 
				? board.ValidShifts.Select(s => AverageSpawnValue(s.Value, remainingDepth - 1)).Max() 
				: board.ValidShifts.Select(s => _evaluator.GetScore(s.Value)).Max();
		}

		private double AverageSpawnValue(Board board, int remainingDepth)
		{
			if (!board.ValidSpawns.Any())
				return 0;
			return remainingDepth > 0 
				? board.ValidSpawns.Sum(s => s.Item1 * BestShiftValue(s.Item2, remainingDepth - 1)) 
				: board.ValidSpawns.Sum(s => s.Item1 * _evaluator.GetScore(s.Item2));
		}
	}
}
