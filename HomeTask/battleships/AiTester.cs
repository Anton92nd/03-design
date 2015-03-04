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

		public List<Game> TestSingleFile(AiProvider aiProvider, MapGenerator gen, GameVisualizer vis,
			int gamesCount, int crashLimit, bool verbose, bool interactive)
		{
			var crashes = 0;
			var games = Enumerable.Range(0, gamesCount)
				.Select(i => aiProvider.TryProvideAi())
				.Where(ai => ai != null)
				.Select(ai => RunGameToEnd(new Game(gen.GenerateMap(), ai), vis, interactive))
				.ToList();
			var gameIndex = 0;
			foreach (var game in games)
			{
				if (game.AiCrashed)
				{
					crashes++;
					if (crashes > crashLimit) break;
				}
				if (verbose)
				{
					Console.WriteLine(
						"Game #{3,4}: Turns {0,4}, BadShots {1}{2}",
						game.TurnsCount, game.BadShots, game.AiCrashed ? ", Crashed" : "", gameIndex);
				}
				gameIndex++;
			}
			return games.Take(gameIndex).ToList();
		}

		private Game RunGameToEnd(Game game, GameVisualizer vis, bool interactive)
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
			return game;
		}
	}
}