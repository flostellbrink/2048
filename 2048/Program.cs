using System;
using Newtonsoft.Json;
using _2048.Core;
using _2048.ScoreFunction;
using _2048.Strategy;

namespace _2048
{
	class Program
	{
		static void Main(string[] args)
		{
			var manual = new ManualStrategy();
			var random = new RandomStrategy();
			var high1Step = new OneStepEvaluatorStrategy(new HighScoreEvaluator());
			var empty1Step = new OneStepEvaluatorStrategy(new HighScoreEvaluator());
			var high10Step = new MaxAverageStrategy(new HighScoreEvaluator(), 4);
			var empty10Step = new MaxAverageStrategy(new HighScoreEvaluator(), 4);

			Demo.RunStrategy(high10Step);

			var result = new
			{
				//manual = runner.TestStrategy(manual, 1),
				random = Runner.TestStrategy(random),
				high1Step = Runner.TestStrategy(high1Step),
				empty1Step = Runner.TestStrategy(empty1Step),
				high10Step = Runner.TestStrategy(high10Step, 5),
				empty10Step = Runner.TestStrategy(empty10Step, 5)
			};
			Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			Console.ReadLine();
		}
	}
}
