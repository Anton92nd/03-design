using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace battleships
{
	public class AiTester
	{
		private static readonly Logger resultsLog = LogManager.GetLogger("results");
		private readonly IMapGenerator gen;
		private readonly IProcessMonitor monitor;
		private readonly IGameVisualizer vis;

		public AiTester(IMapGenerator gen, IProcessMonitor monitor, IGameVisualizer vis)
		{
			this.gen = gen;
			this.monitor = monitor;
			this.vis = vis;
		}

		public void TestSingleFile(string exe, int gamesCount, int crashLimit, 
			bool verbose, bool interactive, int fieldArea)
		{
			var badShots = 0;
			var crashes = 0;
			var gamesPlayed = 0;
			var shots = new List<int>();
			var ai = new Ai(exe, monitor);
			for (var gameIndex = 0; gameIndex < gamesCount; gameIndex++)
			{
				var map = gen.GenerateMap();
				var game = new Game(map, ai);
				RunGameToEnd(game, vis, interactive);
				gamesPlayed++;
				badShots += game.BadShots;
				if (game.AiCrashed)
				{
					crashes++;
					if (crashes > crashLimit) break;
					ai = new Ai(exe, monitor);
				}
				else
					shots.Add(game.TurnsCount);
				if (verbose)
				{
					Console.WriteLine(
						"Game #{3,4}: Turns {0,4}, BadShots {1}{2}",
						game.TurnsCount, game.BadShots, game.AiCrashed ? ", Crashed" : "", gameIndex);
				}
			}
			ai.Dispose();
			WriteTotal(ai, shots, crashes, badShots, gamesPlayed, crashLimit, fieldArea);
		}

		private void RunGameToEnd(Game game, IGameVisualizer vis, bool interactive)
		{
			while (!game.IsOver())
			{
				game.MakeStep();
				if (interactive)
				{
					vis.Visualize(game);
					if (game.AiCrashed)
						Console.WriteLine(game.LastError.Message);
					Console.ReadKey();
				}
			}
		}

		private void WriteTotal(Ai ai, List<int> shots, int crashes, int badShots, int gamesPlayed, int crashLimit, int fieldArea)
		{
			if (shots.Count == 0) shots.Add(1000 * 1000);
			shots.Sort();
			var median = shots.Count % 2 == 1 ? shots[shots.Count / 2] : (shots[shots.Count / 2] + shots[(shots.Count + 1) / 2]) / 2;
			var mean = shots.Average();
			var sigma = Math.Sqrt(shots.Average(s => (s - mean) * (s - mean)));
			var badFraction = (100.0 * badShots) / shots.Sum();
			var crashPenalty = 100.0 * crashes / crashLimit;
			var efficiencyScore = 100.0 * (fieldArea - mean) / fieldArea;
			var score = efficiencyScore - crashPenalty - badFraction;
			var headers = FormatTableRow(new object[] { "AiName", "Mean", "Sigma", "Median", "Crashes", "Bad%", "Games", "Score" });
			var message = FormatTableRow(new object[] { ai.Name, mean, sigma, median, crashes, badFraction, gamesPlayed, score });
			resultsLog.Info(message);
			Console.WriteLine();
			Console.WriteLine("Score statistics");
			Console.WriteLine("================");
			Console.WriteLine(headers);
			Console.WriteLine(message);
		}

		private string FormatTableRow(object[] values)
		{
			return FormatValue(values[0], 15) 
				+ string.Join(" ", values.Skip(1).Select(v => FormatValue(v, 7)));
		}

		private static string FormatValue(object v, int width)
		{
			return v.ToString().Replace("\t", " ").PadRight(width).Substring(0, width);
		}
	}
}