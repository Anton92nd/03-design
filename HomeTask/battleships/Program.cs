using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace battleships
{
	public class Program
	{
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
			var gen = new MapGenerator(settings, new Random(settings.RandomSeed));
			var monitor = new ProcessMonitor(TimeSpan.FromSeconds(settings.TimeLimitSeconds * settings.GamesCount), settings.MemoryLimit);
			var tester = new AiTester(settings, gen, monitor);
			if (File.Exists(aiPath))
				tester.TestSingleFile(aiPath, new GameVisualizer());
			else
				Console.WriteLine("No AI exe-file " + aiPath);
		}
	}
}