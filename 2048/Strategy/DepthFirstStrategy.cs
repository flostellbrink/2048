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
			var results = new Dictionary<int, Direction>();

			// Start running for depth > 1
			var tokenSource = new CancellationTokenSource();
			var outerThread = new Thread(() => RunMoves(board, results, tokenSource.Token));
			outerThread.Start();
			tokenSource.CancelAfter(_limit);

			// Run for depth = 1
			results.Add(1, GetMove(board, 1, tokenSource.Token));

			tokenSource.Token.WaitHandle.WaitOne();
			return results.MaxBy(r => r.Key).Value;
		}

		private void RunMoves(Board board, IDictionary<int, Direction> results, CancellationToken token)
		{
			foreach (var depth in Enumerable.Range(2, int.MaxValue - 2).AsParallel())
			{
				if(token.IsCancellationRequested) return;
				var move = GetMove(board, depth, token);
				if(!token.IsCancellationRequested)
					results.Add(depth, move);
			}
		}

		protected abstract Direction GetMove(Board board, int depth, CancellationToken token);
	}
}
