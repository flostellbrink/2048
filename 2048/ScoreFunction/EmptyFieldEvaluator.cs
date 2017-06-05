using System.Linq;
using _2048.Core;

namespace _2048.ScoreFunction
{
	internal class EmptyFieldEvaluator : IEvaluator
	{
		public double GetScore(Board board)
		{
			return board.Fields.Where(f => f == 0).Count();
		}
	}
}
