using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleships
{
	public interface IGameVisualizer
	{
		void Visualize(Game game);
	}
}
