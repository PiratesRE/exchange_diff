using System;
using System.Threading;

namespace Microsoft.Exchange.Assistants
{
	internal class FastManualResetEvent : IDisposable
	{
		public FastManualResetEvent() : this(false)
		{
		}

		public FastManualResetEvent(bool initialSignaledState)
		{
			if (initialSignaledState)
			{
				this.Set();
			}
		}

		public bool IsSignaled
		{
			get
			{
				return this.isSignaled;
			}
		}

		public void Set()
		{
			this.isSignaled = true;
			lock (this)
			{
				if (this.manualResetEvent != null)
				{
					this.manualResetEvent.Set();
				}
			}
		}

		public void Reset()
		{
			this.isSignaled = false;
			lock (this)
			{
				if (this.manualResetEvent != null)
				{
					this.manualResetEvent.Reset();
				}
			}
		}

		public void WaitOne()
		{
			if (this.isSignaled)
			{
				return;
			}
			this.GetEvent().WaitOne();
		}

		public bool WaitOne(TimeSpan timespan)
		{
			return this.isSignaled || this.GetEvent().WaitOne(timespan, false);
		}

		public ManualResetEvent GetEvent()
		{
			if (this.manualResetEvent == null)
			{
				lock (this)
				{
					if (this.manualResetEvent == null)
					{
						this.manualResetEvent = new ManualResetEvent(this.isSignaled);
					}
				}
			}
			return this.manualResetEvent;
		}

		public void Dispose()
		{
			lock (this)
			{
				if (this.manualResetEvent != null)
				{
					IDisposable disposable = this.manualResetEvent;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.manualResetEvent = null;
				}
			}
		}

		private bool isSignaled;

		private ManualResetEvent manualResetEvent;
	}
}
