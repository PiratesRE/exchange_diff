using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Shared
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class TimerComponent : DisposeTrackableBase, IStartStop
	{
		protected DateTime PrepareToStopTime { get; set; }

		protected DateTime StartOfStopTime { get; set; }

		protected DateTime EndOfStopTime { get; set; }

		public TimerComponent(TimeSpan initialDueTime, TimeSpan period, string name)
		{
			this.m_name = name;
			this.m_initialDueTime = initialDueTime;
			this.m_period = period;
		}

		protected bool PrepareToStopCalled
		{
			get
			{
				return this.m_fShutdown;
			}
		}

		protected TimeSpan Period
		{
			get
			{
				return this.m_period;
			}
		}

		public void Start()
		{
			lock (this)
			{
				if (this.m_fShutdown)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: TimerComponent.Start() skipping since m_fShutdown is true.", this.m_name);
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "{0}: TimerComponent is now starting. m_initialDueTime = {1}, m_period = {2}", this.m_name, this.m_initialDueTime, this.m_period);
					this.m_timer = new Timer(new TimerCallback(this.TimerCallback), null, this.m_initialDueTime, this.m_period);
				}
			}
		}

		public void PrepareToStop()
		{
			lock (this)
			{
				if (this.m_fShutdown)
				{
					return;
				}
				this.m_fShutdown = true;
				this.PrepareToStopTime = DateTime.UtcNow;
			}
			if (this.m_timer != null)
			{
				this.m_timer.Change(-1, -1);
				this.m_disposedEvent = new ManualResetEvent(false);
				this.m_timer.Dispose(this.m_disposedEvent);
			}
		}

		public void ChangeTimer(TimeSpan dueTime, TimeSpan period)
		{
			lock (this)
			{
				if (!this.m_fShutdown)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "{0}: TimerComponent.ChangeTimer(): Changing to: dueTime={1}, period={2}.", this.m_name, dueTime, period);
					this.m_timer.Change(dueTime, period);
				}
			}
		}

		public void StartNow()
		{
			lock (this)
			{
				if (!this.m_fShutdown)
				{
					if (!this.m_fInCallback)
					{
						this.m_fStartNow = false;
						this.m_timer.Change(TimeSpan.Zero, this.m_period);
					}
					else
					{
						this.m_fStartNow = true;
					}
				}
			}
		}

		public void Stop()
		{
			GC.SuppressFinalize(this);
			base.Dispose(true);
		}

		public new void Dispose()
		{
			this.Stop();
		}

		protected virtual void StopInternal()
		{
		}

		public void TimerCallback(object ignoredState)
		{
			lock (this)
			{
				if (this.m_fShutdown)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: TimerCallback() is bailing due to shutdown.", this.m_name);
					return;
				}
				if (this.m_fInCallback)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: TimerCallback() is bailing because another thread is already working.", this.m_name);
					return;
				}
				this.m_fInCallback = true;
			}
			try
			{
				this.TimerCallbackInternal();
			}
			finally
			{
				lock (this)
				{
					this.m_fInCallback = false;
					if (this.m_fStartNow)
					{
						this.StartNow();
					}
				}
			}
		}

		protected abstract void TimerCallbackInternal();

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				bool flag = false;
				lock (this)
				{
					if (!this.m_fShutdown)
					{
						this.PrepareToStop();
					}
					if (!this.m_stopped)
					{
						this.m_stopped = true;
						flag = true;
					}
				}
				if (flag)
				{
					this.StartOfStopTime = DateTime.UtcNow;
					if (this.m_disposedEvent != null)
					{
						try
						{
							this.m_disposedEvent.WaitOne();
						}
						finally
						{
							this.m_disposedEvent.Close();
						}
						this.m_disposedEvent = null;
					}
					this.m_timer = null;
					this.EndOfStopTime = DateTime.UtcNow;
					this.StopInternal();
				}
			}
		}

		protected void LogStopEventAndSetFinalStopTime(string instanceName)
		{
			this.EndOfStopTime = DateTime.UtcNow;
			TimerComponent.LogStopEvent(this.m_name, instanceName, this.PrepareToStopTime, this.StartOfStopTime, this.EndOfStopTime);
		}

		public static void LogStopEvent(string componentName, string instanceName, DateTime prepareToStopTime, DateTime startOfStopTime, DateTime endOfStopTime)
		{
			TimeSpan timeSpan = endOfStopTime - prepareToStopTime;
			ReplayCrimsonEvents.TimerComponentStopped.Log<string, string, TimeSpan, DateTime, DateTime, DateTime>(componentName, instanceName, timeSpan, prepareToStopTime, startOfStopTime, endOfStopTime);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TimerComponent>(this);
		}

		private readonly string m_name;

		private readonly TimeSpan m_initialDueTime;

		private readonly TimeSpan m_period;

		private Timer m_timer;

		private ManualResetEvent m_disposedEvent;

		private bool m_fShutdown;

		private bool m_fInCallback;

		private bool m_stopped;

		private bool m_fStartNow;
	}
}
