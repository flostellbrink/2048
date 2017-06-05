using System.Linq;
using MoreLinq;
using _2048.Core;
using _2048.ScoreFunction;

namespace _2048.Strategy
{
	/// <summary>
	/// Rank next steps by the resulting score according to an <see cref="IEvaluator"/>
	/// </summary>
	internal class OneStepEvaluatorStrategy : IStrategy
	{
		private readonly IEvaluator _evaluator;

		public OneStepEvaluatorStrategy(IEvaluator evaluator)
		{
			_evaluator = evaluator;
		}

		public Direction GetMove(Board board)
		{
			return board.ValidShifts.ToArray().MaxBy(s => _evaluator.GetScore(s.Value)).Key;
		}
	}
}
