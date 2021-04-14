using System;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics
{
	internal class SharedTimer
	{
		private SharedTimer()
		{
			this.internalTimer = new Timer(new TimerCallback(this.InternalCallback), null, 1000, -1);
		}

		internal static SharedTimer Instance
		{
			get
			{
				return SharedTimer.instance;
			}
		}

		internal void RegisterCallback(TimerCallback callback)
		{
			lock (this.sync)
			{
				this.callback = (TimerCallback)Delegate.Combine(this.callback, callback);
			}
		}

		internal void UnRegisterCallback(TimerCallback callback)
		{
			lock (this.sync)
			{
				this.callback = (TimerCallback)Delegate.Remove(this.callback, callback);
			}
		}

		private void InternalCallback(object arg)
		{
			TimerCallback timerCallback = null;
			lock (this.sync)
			{
				timerCallback = this.callback;
			}
			if (timerCallback != null)
			{
				timerCallback(null);
			}
			this.internalTimer.Change(1000, -1);
		}

		private const int TimerIntervalMilliseconds = 1000;

		private static SharedTimer instance = new SharedTimer();

		private Timer internalTimer;

		private TimerCallback callback;

		private object sync = new object();
	}
}
