using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Threading
{
	public sealed class GuardedTimer : IGuardedTimer, IDisposeTrackable, IDisposable
	{
		public GuardedTimer(TimerCallback timerCallback, object state, TimeSpan dueTime, TimeSpan period) : this(timerCallback, state, (long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds)
		{
		}

		public GuardedTimer(TimerCallback timerCallback)
		{
			this.syncRoot = new object();
			base..ctor();
			if (timerCallback == null)
			{
				throw new ArgumentNullException("timerCallback");
			}
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.timerCallback = timerCallback;
			this.period = -1L;
			using (ActivityContext.SuppressThreadScope())
			{
				this.timer = new Timer(new TimerCallback(this.TimerGuardedCallback), this, -1, -1);
			}
		}

		public GuardedTimer(TimerCallback timerCallback, object state, int dueTime, int period) : this(timerCallback, state, (long)dueTime, (long)period)
		{
		}

		public GuardedTimer(TimerCallback timerCallback, object state, long dueTime, long period)
		{
			this.syncRoot = new object();
			base..ctor();
			if (timerCallback == null)
			{
				throw new ArgumentNullException("timerCallback");
			}
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.timerCallback = timerCallback;
			this.period = period;
			using (ActivityContext.SuppressThreadScope())
			{
				this.timer = new Timer(new TimerCallback(this.TimerGuardedCallback), state, dueTime, period);
			}
		}

		public GuardedTimer(TimerCallback timerCallback, object state, TimeSpan period) : this(timerCallback, state, period, period)
		{
		}

		public bool Change(int dueTime, int period)
		{
			return this.Change((long)dueTime, (long)period);
		}

		public bool Change(long dueTime, long period)
		{
			bool result;
			using (ActivityContext.SuppressThreadScope())
			{
				if (this.timer != null && !this.disposing)
				{
					this.period = period;
					result = this.timer.Change(dueTime, period);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool Change(TimeSpan dueTime, TimeSpan period)
		{
			return this.Change((long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds);
		}

		public void Pause()
		{
			using (ActivityContext.SuppressThreadScope())
			{
				this.ThrowIfDisposed();
				this.timer.Change(-1, -1);
			}
		}

		public void Continue()
		{
			using (ActivityContext.SuppressThreadScope())
			{
				this.ThrowIfDisposed();
				if (!this.disposing)
				{
					this.timer.Change(this.period, this.period);
				}
			}
		}

		public void Continue(TimeSpan dueTime, TimeSpan period)
		{
			using (ActivityContext.SuppressThreadScope())
			{
				this.ThrowIfDisposed();
				this.period = (long)period.TotalMilliseconds;
				if (!this.disposing)
				{
					this.timer.Change(dueTime, period);
				}
			}
		}

		public override bool Equals(object obj)
		{
			this.ThrowIfDisposed();
			return this.timer.Equals(obj);
		}

		public override int GetHashCode()
		{
			this.ThrowIfDisposed();
			return this.timer.GetHashCode();
		}

		public override string ToString()
		{
			this.ThrowIfDisposed();
			return this.timer.ToString();
		}

		public void Dispose(bool wait)
		{
			this.ThrowIfDisposed();
			this.guard = true;
			if (wait)
			{
				this.disposing = true;
				this.Pause();
				lock (this.syncRoot)
				{
					ManualResetEvent manualResetEvent = new ManualResetEvent(false);
					try
					{
						if (this.timer.Dispose(manualResetEvent) && !Environment.HasShutdownStarted)
						{
							manualResetEvent.WaitOne();
						}
					}
					finally
					{
						manualResetEvent.Close();
					}
					goto IL_71;
				}
			}
			this.timer.Dispose();
			IL_71:
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.timer = null;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<GuardedTimer>(this);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void TimerGuardedCallback(object state)
		{
			try
			{
				if (Monitor.TryEnter(this.syncRoot) && !this.guard)
				{
					using (ActivityContext.SuppressThreadScope())
					{
						this.timerCallback(state);
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.syncRoot))
				{
					Monitor.Exit(this.syncRoot);
				}
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.timer == null)
			{
				throw new ObjectDisposedException("Timer already disposed.");
			}
		}

		private Timer timer;

		private DisposeTracker disposeTracker;

		private TimerCallback timerCallback;

		private long period;

		private bool guard;

		private object syncRoot;

		private bool disposing;
	}
}
