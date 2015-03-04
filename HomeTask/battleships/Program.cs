using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;
using NUnit.Framework;

namespace battleships
{
	public class Program
	{
		private static readonly Logger resultsLog = LogManager.GetLogger("results");

		private static void Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: {0} <ai.exe>", Process.GetCurrentProcess().ProcessName);
				return;
			}
			var aiPath = args[0];
			var settings = new Settings("settings.txt");
			var tester = new AiTester();
			var monitor = new ProcessMonitor(TimeSpan.FromSeconds(settings.TimeLimitSeconds * settings.GamesCount), settings.MemoryLimit);
			var gen = new MapGenerator(new Random(settings.RandomSeed), settings.Width, settings.Height, settings.Ships);
			var vis = new GameVisualizer();
			var aiProvider = new AiProvider(monitor, aiPath);
			if (File.Exists(aiPath))
			{
				var results = tester.TestSingleFile(aiProvider, gen, vis, settings.GamesCount, 
					settings.CrashLimit, settings.Verbose, settings.Interactive);
				WriteTotalStats(new Ai(aiPath).Name, results, settings.CrashLimit, settings.Width, settings.Height);
			}
			else
				Console.WriteLine("No AI exe-file " + aiPath);
		}

		private static void WriteTotalStats(string aiName, List<Game> games, int crashLimit, int width, int height)
		{
			var shots = games.Where(g => !g.AiCrashed).Select(g => g.TurnsCount).ToList();
			var badShots = games.Select(g => g.BadShots).Sum();
			var crashes = games.Count(g => g.AiCrashed);
			var gamesPlayed = games.Count;
			if (shots.Count == 0) shots.Add(1000 * 1000);
			shots.Sort();
			var median = shots.Count % 2 == 1 ? shots[shots.Count / 2] : (shots[shots.Count / 2] + shots[(shots.Count + 1) / 2]) / 2;
			var mean = shots.Average();
			var sigma = Math.Sqrt(shots.Average(s => (s - mean) * (s - mean)));
			var badFraction = (100.0 * badShots) / shots.Sum();
			var crashPenalty = 100.0 * crashes / crashLimit;
			var efficiencyScore = 100.0 * (width * height - mean) / (width * height);
			var score = efficiencyScore - crashPenalty - badFraction;
			var headers = new object[] { "AiName", "Mean", "Sigma", "Median", "Crashes", "Bad%", "Games", "Score" };
			var values = new object[] { aiName, mean, sigma, median, crashes, badFraction, gamesPlayed, score };
			var headersString = FormatTableRow(headers);
			var messageString = FormatTableRow(values);
			resultsLog.Info(messageString);
			Console.WriteLine();
			Console.WriteLine("Score statistics");
			Console.WriteLine("================");
			Console.WriteLine(headersString);
			Console.WriteLine(messageString);
		}

		private static string FormatTableRow(object[] values)
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