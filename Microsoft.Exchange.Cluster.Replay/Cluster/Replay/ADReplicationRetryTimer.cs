using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADReplicationRetryTimer : RetryTimer
	{
		public static TimeSpan GetMaxWait()
		{
			return new TimeSpan(0, 0, RegistryParameters.MaxADReplicationWaitInSec);
		}

		public static TimeSpan GetSleepTime()
		{
			return new TimeSpan(0, 0, RegistryParameters.ADReplicationSleepInSec);
		}

		public ADReplicationRetryTimer() : base(ADReplicationRetryTimer.GetMaxWait())
		{
		}

		public override TimeSpan SleepTime
		{
			get
			{
				return ADReplicationRetryTimer.GetSleepTime();
			}
		}
	}
}
