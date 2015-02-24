using System;
using System.Threading;

namespace DIContainer.Commands
{
	public class HelpCommand : BaseCommand
	{
		private readonly Lazy<ICommand[]> commands;

		public HelpCommand(Lazy<ICommand[]> commands)
		{
			this.commands = commands;
		}

		public override void Execute()
		{
			foreach (var i in commands.Value)
			{
				Console.WriteLine(i.ToString());
			}
		}
	}
}