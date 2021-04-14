using System;
using System.Threading;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class ShutdownMarker
	{
		internal bool IsShutdown
		{
			get
			{
				return this.m_isShutdown;
			}
		}

		internal bool Enter()
		{
			lock (this.m_locker)
			{
				if (this.m_isShutdown)
				{
					return false;
				}
				if (this.m_isInUse)
				{
					return false;
				}
				this.m_isInUse = true;
			}
			return true;
		}

		internal void Leave()
		{
			lock (this.m_locker)
			{
				this.m_isInUse = false;
				if (this.m_doneEvent != null)
				{
					this.m_doneEvent.Set();
				}
			}
		}

		internal void TriggerShutdownAndWait()
		{
			lock (this.m_locker)
			{
				this.m_isShutdown = true;
				if (this.m_isInUse)
				{
					this.m_doneEvent = new AutoResetEvent(false);
				}
			}
			if (this.m_doneEvent != null)
			{
				this.m_doneEvent.WaitOne();
				this.m_doneEvent.Close();
				this.m_doneEvent = null;
			}
		}

		private object m_locker = new object();

		private bool m_isShutdown;

		private bool m_isInUse;

		private AutoResetEvent m_doneEvent;
	}
}
