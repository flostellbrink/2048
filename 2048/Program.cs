using System;
using Newtonsoft.Json;
using _2048.Core;
using _2048.ScoreFunction;
using _2048.Strategy;

namespace _2048
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var manual = new ManualStrategy();
			var random = new RandomStrategy();
			var high1Step = new OneStepEvaluatorStrategy(new HighScoreEvaluator());
			var empty1Step = new OneStepEvaluatorStrategy(new HighScoreEvaluator());
			var highMultiStep = new MaxAverageStrategy(new HighScoreEvaluator(), TimeSpan.FromMilliseconds(100));
			var emptyMultiStep = new MaxAverageStrategy(new HighScoreEvaluator(), TimeSpan.FromMilliseconds(100));

			//Demo.RunStrategy(highMultiStep);

			var result = new
			{
				//manual = runner.TestStrategy(manual, 1),
				random = Runner.TestStrategy(random),
				high1Step = Runner.TestStrategy(high1Step),
				empty1Step = Runner.TestStrategy(empty1Step),
				highMultiStep = Runner.TestStrategy(highMultiStep, 1),
				emptyMultiStep = Runner.TestStrategy(emptyMultiStep, 1)
			};
			Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			Console.ReadLine();
		}
	}
}
