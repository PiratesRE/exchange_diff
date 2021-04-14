using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface ISchedulerMetering
	{
		UsageData GetTotalUsage();

		void RecordStart(IEnumerable<IMessageScope> scopes, long memorySize);

		void RecordEnd(IEnumerable<IMessageScope> scopes, TimeSpan duration);

		bool TryGetUsage(IMessageScope scope, out UsageData data);
	}
}
