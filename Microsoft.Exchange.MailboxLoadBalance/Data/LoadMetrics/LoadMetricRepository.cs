using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadMetricRepository
	{
		public static readonly IEnumerable<LoadMetric> DefaultMetrics = new LoadMetric[]
		{
			ConsumedCpu.Instance,
			ItemCount.Instance,
			LogicalSize.Instance,
			PhysicalSize.Instance,
			ConsumerMailboxCount.Instance,
			ConsumerMailboxSize.Instance,
			InProgressLoadBalancingMoveCount.Instance
		};
	}
}
