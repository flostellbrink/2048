using _2048.Core;

namespace _2048
{
	internal interface IStrategy
	{
		Direction GetMove(Board board);
	}
}
