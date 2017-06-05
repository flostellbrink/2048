using System;
using System.Linq;

namespace _2048.Core
{
	internal static class Demo
	{
		internal static void RunStrategy(IStrategy strategy)
		{
			(var running, var board) = Board.Empty.TrySpawn();
			Console.WriteLine($"Starting game with {strategy}");
			while (running)
			{
				Console.WriteLine(board);
				if (!board.ValidShifts.Any()) break;
				Console.WriteLine("Press any key");
				Console.ReadKey();
				Console.WriteLine("Running...");
				var direction = strategy.GetMove(board);
				board = board.Shift(direction);
				(running, board) = board.TrySpawn();
				Console.Clear();
				Console.WriteLine(direction.ToString());
			}
			Console.Clear();
			Console.WriteLine("Game Over");
			Console.WriteLine(board);
			Console.ReadLine();
		}
	}
}
