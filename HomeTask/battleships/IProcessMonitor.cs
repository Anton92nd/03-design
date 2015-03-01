using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleships
{
	public interface IProcessMonitor
	{
		void Register(Process process);
	}
}
