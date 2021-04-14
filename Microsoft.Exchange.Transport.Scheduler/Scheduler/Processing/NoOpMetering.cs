using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class NoOpMetering : ISchedulerMetering
	{
		public UsageData GetTotalUsage()
		{
			return this.totalUsage;
		}

		public void RecordStart(IEnumerable<IMessageScope> scopes, long memorySize)
		{
		}

		public void RecordEnd(IEnumerable<IMessageScope> scopes, TimeSpan duration)
		{
		}

		public bool TryGetUsage(IMessageScope scope, out UsageData data)
		{
			data = null;
			return false;
		}

		private readonly UsageData totalUsage = new UsageData(0, 0L, 0L);
	}
}
