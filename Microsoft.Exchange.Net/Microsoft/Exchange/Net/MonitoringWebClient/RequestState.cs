using System;
using System.Threading;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class RequestState
	{
		public int State
		{
			get
			{
				return this.state;
			}
		}

		public bool IsCancelled
		{
			get
			{
				return this.state == 1;
			}
		}

		public RequestState(TimerCallback timerCallback, object asyncState, int dueTime) : this(timerCallback, asyncState, dueTime, true)
		{
		}

		public RequestState(TimerCallback timerCallback, object asyncState, int dueTime, bool startTimer)
		{
			this.timerCallback = timerCallback;
			this.timerAsyncState = asyncState;
			this.dueTime = dueTime;
			if (startTimer)
			{
				this.StartTimer();
			}
		}

		public void StartTimer()
		{
			this.timer = new Timer(this.timerCallback, this.timerAsyncState, this.dueTime, 0);
		}

		public bool TryTransitionFromExecutingToTimedOut()
		{
			Interlocked.CompareExchange(ref this.state, 1, 0);
			this.DisposeTimer();
			return this.state == 1;
		}

		public bool TryTransitionFromExecutingToFinished()
		{
			Interlocked.CompareExchange(ref this.state, 2, 0);
			this.DisposeTimer();
			return this.state == 2;
		}

		public void Cancel()
		{
			if (Interlocked.CompareExchange(ref this.state, 1, 0) == 0)
			{
				this.DisposeTimer();
				this.timerCallback(this.timerAsyncState);
			}
		}

		private void DisposeTimer()
		{
			Timer timer = Interlocked.Exchange<Timer>(ref this.timer, null);
			if (timer != null)
			{
				timer.Dispose();
			}
		}

		private int state;

		private Timer timer;

		private TimerCallback timerCallback;

		private readonly int dueTime;

		private object timerAsyncState;
	}
}
