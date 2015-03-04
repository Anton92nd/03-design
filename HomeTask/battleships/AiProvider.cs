using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleships
{
	public class AiProvider
	{
		private readonly ProcessMonitor monitor;
		private readonly string aiPath;

		public AiProvider(ProcessMonitor monitor, string aiPath)
		{
			this.monitor = monitor;
			this.aiPath = aiPath;
		}

		public Ai TryProvideAi()
		{
			try
			{
				var result = new Ai(aiPath);
				result.ProcessStarted += monitor.Register;
				return result;
			}
			catch
			{
				return null;
			}
		}
	}
}
