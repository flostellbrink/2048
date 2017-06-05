using _2048.Core;

namespace _2048.ScoreFunction
{
	class HighScoreEvaluator : IEvaluator
	{
		public double GetScore(Board board)
		{
			return board.Score;
		}
	}
}
