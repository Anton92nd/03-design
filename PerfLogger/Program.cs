using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace PerfLogger
{
	class Program
	{
		public class PerfLogger : IDisposable
		{
 		
			Stopwatch sw = new Stopwatch();
			private string message;

			public PerfLogger(string message)
			{
				this.message = message;
				sw.Start();
			}

			public void Dispose()
			{
				sw.Stop();
				Console.WriteLine(sw.Elapsed + " " + message);
			}


			public static IDisposable Measure(string messageToWrite)
			{
				var perfLogger = new PerfLogger(messageToWrite);
				return perfLogger;
			}
		}

		static void Main(string[] args)
		{
			var sum = 0.0;
			using (PerfLogger.Measure("100M for iterations"))
				for (var i = 0; i < 100000000; i++) sum += i;
			using (PerfLogger.Measure("100M LINQ iterations"))
				sum -= Enumerable.Range(0, 100000000).Sum(i => (double)i);
			Console.WriteLine(sum);
			Console.ReadKey();
		}


	}
}
