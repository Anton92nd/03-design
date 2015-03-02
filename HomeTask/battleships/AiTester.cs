using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NLog;

namespace battleships
{
	public class AiTester
	{
		public AiTester()
		{
		}

		private void RestartAi(string exe, out Ai ai, ProcessMonitor monitor)
		{
			ai = new Ai(exe);
			ai.processStarted += monitor.Register;
		}


		public GameResults TestSingleFile(string exe, MapGenerator gen, ProcessMonitor monitor, GameVisualizer vis,
			int height, int width, int gamesCount, int crashLimit, bool verbose, bool interactive)
		{
			var crashes = 0;
			var games = new List<Game>();
			Ai ai;
			RestartAi(exe, out ai, monitor);
			for (var gameIndex = 0; gameIndex < gamesCount; gameIndex++)
			{
				var map = gen.GenerateMap();
				var game = new Game(map, ai);
				RunGameToEnd(game, vis, interactive);
				if (game.AiCrashed)
				{
					crashes++;
					if (crashes > crashLimit) break;
					RestartAi(exe, out ai, monitor);
				}
				games.Add(game);
				if (verbose)
				{
					Console.WriteLine(
						"Game #{3,4}: Turns {0,4}, BadShots {1}{2}",
						game.TurnsCount, game.BadShots, game.AiCrashed ? ", Crashed" : "", gameIndex);
				}
			}
			ai.Dispose();
			return new GameResults(ai.Name, games, crashLimit, width, height);
		}

		private void RunGameToEnd(Game game, GameVisualizer vis, bool interactive)
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
	}
}