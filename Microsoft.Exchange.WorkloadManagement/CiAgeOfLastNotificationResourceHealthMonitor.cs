using System;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class CiAgeOfLastNotificationResourceHealthMonitor : CiResourceHealthMonitorBase
	{
		internal CiAgeOfLastNotificationResourceHealthMonitor(CiAgeOfLastNotificationResourceKey key) : base(key)
		{
		}

		protected override int GetMetricFromStatusInternal(RpcDatabaseCopyStatus2 status)
		{
			int? contentIndexBacklog = status.ContentIndexBacklog;
			if (contentIndexBacklog == null)
			{
				return -1;
			}
			return contentIndexBacklog.GetValueOrDefault();
		}
	}
}
