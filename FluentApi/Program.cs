using System;
using System.Collections.Generic;
using System.Threading;

namespace FluentTask
{
    internal class Program
    {
	    private class Behavior
	    {
		    private List<Action> actions = new List<Action>();

		    public Behavior Say(string s)
		    {
			    actions.Add(() => Console.WriteLine(s));
			    actions.Add(() => Thread.Sleep(200));
			    return this;
			}

		    public Behavior Jump(JumpHeight height)
		    {
				actions.Add(() =>
				{
					Console.WriteLine("[jumped " + height.ToString() + "]");
					Thread.Sleep(200);
				});
			    return this;
		    }

		    public Behavior Delay(TimeSpan delay)
		    {
			    actions.Add(() => Thread.Sleep(delay));
			    return this;
		    }

		    public Behavior UntilKeyPressed(Func<Behavior, Behavior> b)
		    {
				actions.Add(() =>
				{
					while (!Console.KeyAvailable)
					{
						b(new Behavior()).Execute();
					}
					Console.ReadKey();
					Console.Write("\r");
				});
			    return this;
		    }

		    public void Execute()
		    {
			    actions.ForEach(a => a());
		    }

	    }


        private static void Main()
        {
	        var behaviour = new Behavior()
		        .Say("Привет мир!")
                .UntilKeyPressed(b => b
                    .Say("Ля-ля-ля!")
                    .Say("Тру-лю-лю"))
                .Jump(JumpHeight.High)
                .UntilKeyPressed(b => b
                    .Say("Aa-a-a-a-aaaaaa!!!")
                    .Say("[набирает воздух в легкие]"))
                .Say("Ой!")
                .Delay(TimeSpan.FromSeconds(1))
                .Say("Кто здесь?!")
                .Delay(TimeSpan.FromMilliseconds(2000));
			   
            behaviour.Execute();
        }
    }
}