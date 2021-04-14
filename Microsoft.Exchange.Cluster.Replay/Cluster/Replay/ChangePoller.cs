using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class ChangePoller : IStartStop
	{
		public ChangePoller(bool fMakeShutdownEvent)
		{
			if (fMakeShutdownEvent)
			{
				this.m_shutdownEvent = new ManualResetEvent(false);
			}
		}

		public bool StopCalled
		{
			get
			{
				return this.m_fShutdown;
			}
		}

		public void Start()
		{
			lock (this)
			{
				this.m_pollerThread = new Thread(new ThreadStart(this.PollerThread));
				this.m_pollerThread.Start();
			}
		}

		public virtual void PrepareToStop()
		{
			this.m_fShutdown = true;
			if (this.m_shutdownEvent != null)
			{
				this.m_shutdownEvent.Set();
			}
		}

		public virtual void Stop()
		{
			if (!this.m_fShutdown)
			{
				this.PrepareToStop();
			}
			lock (this)
			{
				if (this.m_pollerThread != null)
				{
					this.m_pollerThread.Join();
				}
				if (this.m_shutdownEvent != null)
				{
					this.m_shutdownEvent.Close();
					this.m_shutdownEvent = null;
				}
			}
		}

		protected abstract void PollerThread();

		protected bool m_fShutdown;

		protected ManualResetEvent m_shutdownEvent;

		private Thread m_pollerThread;
	}
}
