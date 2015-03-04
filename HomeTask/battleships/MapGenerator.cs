using System;
using System.Linq;
using NUnit.Framework;

namespace battleships
{
	public class MapGenerator
	{
		private readonly Random random;
		private readonly int width, height;
		private readonly int[] ships;

		public MapGenerator(Random random, int width, int height, int[] ships)
		{
			this.random = random;
			this.width = width;
			this.height = height;
			this.ships = ships;
		}

		public Map GenerateMap()
		{
			var map = new Map(width, height);
			foreach (var size in ships.OrderByDescending(s => s))
				PlaceShip(map, size);
			return map;
		}

		private void PlaceShip(Map map, int size)
		{
			var cells = Vector.Rect(0, 0, width, height).OrderBy(v => random.Next());
			foreach (var loc in cells)
			{
				var horizontal = random.Next(2) == 0;
				if (map.Set(loc, size, horizontal) || map.Set(loc, size, !horizontal)) return;
			}
			throw new Exception("Can't put next ship on map. No free space");
		}
	}

	[TestFixture]
	public class MapGenerator_should
	{
		[Test]
		public void always_succeed_on_standard_map()
		{
			var ships = new[] { 1, 1, 1, 1, 2, 2, 2, 3, 3, 4 };
			var gen = new MapGenerator(new Random(), 10, 10, ships);
			for (var i = 0; i < 10000; i++)
				gen.GenerateMap();
		}
	}
}