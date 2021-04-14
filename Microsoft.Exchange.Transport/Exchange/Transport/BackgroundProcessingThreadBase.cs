using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal abstract class BackgroundProcessingThreadBase
	{
		protected BackgroundProcessingThreadBase() : this(TimeSpan.FromMilliseconds(100.0))
		{
		}

		protected BackgroundProcessingThreadBase(TimeSpan interval)
		{
			if (interval < TimeSpan.FromMilliseconds(100.0))
			{
				throw new ArgumentException("Background thread interval must be greater than or equal to 100ms.");
			}
			this.interval = interval;
			this.hangDetectionInterval = Components.TransportAppConfig.WorkerProcess.BackgroundProcessingThreadHangDetectionToleranceInterval + this.interval;
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			this.startupServiceState = new ServiceState?(targetRunningState);
			this.backgroundThread = new Thread(new ThreadStart(this.Run));
			this.backgroundThread.Start();
			this.lastProcessingTime = DateTime.UtcNow;
			Thread thread = new Thread(new ThreadStart(this.DetectBackgroundProcessingThreadHang));
			thread.Start();
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Background processing thread started");
		}

		public virtual void Stop()
		{
			if (this.backgroundThread != null)
			{
				this.hangDetectionThreadShutdownEvent.Set();
				this.backgroundShutdownEvent.Set();
				this.backgroundThread.Join();
				this.backgroundThread = null;
				ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Background processing thread stopped");
				this.backgroundShutdownEvent.Reset();
			}
		}

		protected virtual void Run()
		{
			WaitHandle[] waitHandles = new WaitHandle[]
			{
				this.backgroundShutdownEvent
			};
			int num;
			for (;;)
			{
				num = WaitHandle.WaitAny(waitHandles, this.interval, false);
				if (num == 0)
				{
					break;
				}
				if (num != 258)
				{
					goto IL_44;
				}
				this.lastProcessingTime = DateTime.UtcNow;
				this.RunMain(DateTime.UtcNow);
			}
			return;
			IL_44:
			throw new InvalidOperationException("Unexpected WaitHandle index: " + num.ToString());
		}

		protected abstract void RunMain(DateTime utcNow);

		private void DetectBackgroundProcessingThreadHang()
		{
			while (!this.hangDetectionThreadShutdownEvent.WaitOne(this.hangDetectionInterval))
			{
				TimeSpan timeSpan = DateTime.UtcNow - this.lastProcessingTime;
				if (timeSpan > this.hangDetectionInterval)
				{
					throw new BackgroundProcessingThreadBase.BackgroundProcessingThreadBlockedException("Background processing is Hung for " + timeSpan);
				}
			}
			ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Exiting DetectBackgroundProcessingThread Hang thread");
		}

		private readonly TimeSpan interval;

		private readonly TimeSpan hangDetectionInterval;

		private Thread backgroundThread;

		private AutoResetEvent backgroundShutdownEvent = new AutoResetEvent(false);

		private AutoResetEvent hangDetectionThreadShutdownEvent = new AutoResetEvent(false);

		private DateTime lastProcessingTime;

		protected ServiceState? startupServiceState = null;

		private class BackgroundProcessingThreadBlockedException : Exception
		{
			public BackgroundProcessingThreadBlockedException(string message) : base(message)
			{
			}
		}
	}
}
