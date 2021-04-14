using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RetryTimer
	{
		public RetryTimer(TimeSpan maxWait)
		{
			this.m_expiryTime = DateTime.UtcNow.Add(maxWait);
		}

		public bool IsExpired
		{
			get
			{
				return DateTime.UtcNow >= this.m_expiryTime;
			}
		}

		public virtual TimeSpan SleepTime
		{
			get
			{
				return new TimeSpan(0, 0, 3);
			}
		}

		public void Sleep()
		{
			Thread.Sleep(this.SleepTime);
		}

		private DateTime m_expiryTime;
	}
}
