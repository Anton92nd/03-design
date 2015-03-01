using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Ninject;

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
			var kernel = new StandardKernel();
			var settings = new Settings("settings.txt");
			kernel.Bind<IMapGenerator>().To<MapGenerator>()
				.WithConstructorArgument(settings)
				.WithConstructorArgument(new Random(settings.RandomSeed));
			//var gen = new MapGenerator(settings, new Random(settings.RandomSeed));
			kernel.Bind<IProcessMonitor>().To<ProcessMonitor>()
				.WithConstructorArgument(TimeSpan.FromSeconds(settings.TimeLimitSeconds*settings.GamesCount))
				.WithConstructorArgument((long)settings.MemoryLimit);
			//var monitor = new ProcessMonitor(TimeSpan.FromSeconds(settings.TimeLimitSeconds * settings.GamesCount), settings.MemoryLimit);
			kernel.Bind<AiTester>().To<AiTester>().WithConstructorArgument(settings);
			//var tester = new AiTester(settings, gen, monitor);
			kernel.Bind<IGameVisualizer>().To<GameVisualizer>();
			if (File.Exists(aiPath))
			{
				kernel.Get<AiTester>().TestSingleFile(aiPath, settings.GamesCount, settings.CrashLimit,
					settings.Verbose, settings.Interactive, settings.Width * settings.Height);
				//tester.TestSingleFile(aiPath, new GameVisualizer());
			}
			else
				Console.WriteLine("No AI exe-file " + aiPath);
		}
	}
}