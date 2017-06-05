using System;
using System.Diagnostics;
using System.Linq;
using _2048.Core;

namespace _2048
{
	class RandomStrategy : IStrategy
	{
		static Random random = new Random();

		public Direction GetMove(Board board)
		{
			return board.ValidShifts.ToArray()[random.Next(board.ValidShifts.Count)].Key;
		}
	}
}