using System;
using System.Linq;
using _2048.Strategy;

namespace _2048.Core
{
	internal static class Demo
	{
		internal static void RunStrategy(IStrategy strategy)
		{
			var board = Board.Empty.Spawn();
			Console.WriteLine($"Starting game with {strategy}");
			while (true)
			{
				Console.WriteLine(board);
				if (!board.ValidShifts.Any()) break;
				Console.WriteLine("Press any key");
				Console.ReadKey();
				Console.WriteLine("Running...");
				var direction = strategy.GetMove(board);
				board = board.Shift(direction, true);
				if(!board.ValidSpawns.Any()) break;
				board = board.Spawn(true);
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
