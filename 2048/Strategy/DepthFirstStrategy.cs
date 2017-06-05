using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MoreLinq;
using _2048.Core;

namespace _2048.Strategy
{
	/// <summary>
	/// Wraps a depth first strategy to run as breadth first with time limit
	/// </summary>
	internal abstract class DepthFirstStrategy : IStrategy
	{
		private readonly TimeSpan _limit;

		protected DepthFirstStrategy(TimeSpan limit)
		{
			_limit = limit;
		}

		public Direction GetMove(Board board)
		{
			// Keep track of time used for thread initialization and level 1 search
			var depth1Watch = Stopwatch.StartNew();
			var results = new Dictionary<int, Direction>();

			// Start running for depth > 1
			var outerThread = new Thread(() => RunMoves(board, results));
			outerThread.Start();

			// Run for depth = 1
			results.Add(1, GetMove(board, 1));

			// Keep thread running for remaining time
			var remaining = _limit - depth1Watch.Elapsed;
			if (remaining > TimeSpan.Zero)
				Thread.Sleep(remaining);

			// Abort thread and return best result
			outerThread.Abort();
			return results.MaxBy(r => r.Key).Value;
		}

		private void RunMoves(Board board, IDictionary<int, Direction> results)
		{
			foreach (var depth in Enumerable.Range(2, int.MaxValue - 2).AsParallel())
			{
				results.Add(depth, GetMove(board, depth));
			}
		}

		protected abstract Direction GetMove(Board board, int depth);
	}
}
