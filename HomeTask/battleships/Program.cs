using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;

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
			if (File.Exists(aiPath))
			{
				var results = tester.TestSingleFile(aiPath, gen, monitor, vis, settings.Height, settings.Width, 
					settings.GamesCount, settings.CrashLimit, settings.Verbose, settings.Interactive);
				WriteTotalStats(results);
			}
			else
				Console.WriteLine("No AI exe-file " + aiPath);
		}

		private static void WriteTotalStats(GameResults stats)
		{
			var headers = FormatTableRow(stats.Headers);
			var message = FormatTableRow(stats.Values);
			resultsLog.Info(message);
			Console.WriteLine();
			Console.WriteLine("Score statistics");
			Console.WriteLine("================");
			Console.WriteLine(headers);
			Console.WriteLine(message);
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