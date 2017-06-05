using _2048.Core;

namespace _2048.Strategy
{
	internal interface IStrategy
	{
		Direction GetMove(Board board);
	}
}
