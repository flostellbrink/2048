using _2048.Core;

namespace _2048
{
	interface IStrategy
	{
		Direction GetMove(Board board);
	}
}
