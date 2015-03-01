using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleships
{
	public class GameResults
	{
		public object[] Headers { get; private set; }
		public object[] Values { get; private set; }

		public GameResults(string aiName, List<int> shots, int crashes, int badShots, int gamesPlayed, int crashLimit, int width, int height)
		{
			Headers = new object[] { "AiName", "Mean", "Sigma", "Median", "Crashes", "Bad%", "Games", "Score" };
			if (shots.Count == 0) shots.Add(1000 * 1000);
			shots.Sort();
			var median = shots.Count % 2 == 1 ? shots[shots.Count / 2] : (shots[shots.Count / 2] + shots[(shots.Count + 1) / 2]) / 2;
			var mean = shots.Average();
			var sigma = Math.Sqrt(shots.Average(s => (s - mean) * (s - mean)));
			var badFraction = (100.0 * badShots) / shots.Sum();
			var crashPenalty = 100.0 * crashes / crashLimit;
			var efficiencyScore = 100.0 * (width * height - mean) / (width * height);
			var score = efficiencyScore - crashPenalty - badFraction;
			Values = new object[] { aiName, mean, sigma, median, crashes, badFraction, gamesPlayed, score };
		}
	}
}
