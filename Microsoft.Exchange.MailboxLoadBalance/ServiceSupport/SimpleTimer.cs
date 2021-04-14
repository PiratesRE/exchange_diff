using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SimpleTimer : ITimer
	{
		private SimpleTimer(TimeSpan interval)
		{
			this.interval = interval;
			this.executionSignal = new ManualResetEventSlim();
		}

		public static ITimer CreateTimer(TimeSpan interval)
		{
			return SimpleTimer.Factory.Value(interval);
		}

		public void SetAction(Action newAction, bool startExecution)
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
			}
			this.action = newAction;
			this.timer = new Timer(new TimerCallback(this.RunAction), null, startExecution ? TimeSpan.Zero : this.interval, this.interval);
			if (startExecution)
			{
				this.WaitExecution();
			}
		}

		public void WaitExecution()
		{
			this.executionSignal.Wait();
		}

		public void WaitExecution(TimeSpan timeout)
		{
			this.executionSignal.Wait(timeout);
		}

		private static ITimer CreateSimpleTimer(TimeSpan interval)
		{
			return new SimpleTimer(interval);
		}

		private void RunAction(object state)
		{
			this.executionSignal.Reset();
			try
			{
				this.action();
			}
			finally
			{
				this.executionSignal.Set();
			}
		}

		internal static readonly Hookable<Func<TimeSpan, ITimer>> Factory = Hookable<Func<TimeSpan, ITimer>>.Create(true, new Func<TimeSpan, ITimer>(SimpleTimer.CreateSimpleTimer));

		private readonly ManualResetEventSlim executionSignal;

		private readonly TimeSpan interval;

		private Action action;

		private Timer timer;
	}
}
