using _2048.Core;

namespace _2048.ScoreFunction
{
	internal interface IEvaluator
	{
		double GetScore(Board board);
	}
}
