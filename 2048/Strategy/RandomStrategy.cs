using System;
using System.Linq;
using _2048.Core;

namespace _2048.Strategy
{
	internal class RandomStrategy : IStrategy
	{
		private static Random random = new Random();

		public Direction GetMove(Board board)
		{
			return board.ValidShifts.ToArray()[random.Next(board.ValidShifts.Count)].Key;
		}
	}
}