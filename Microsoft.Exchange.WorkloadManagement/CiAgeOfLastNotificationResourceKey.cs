using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class CiAgeOfLastNotificationResourceKey : CiResourceKey
	{
		public CiAgeOfLastNotificationResourceKey(Guid mdbGuid) : base(ResourceMetricType.CiAgeOfLastNotification, mdbGuid)
		{
		}

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new CiAgeOfLastNotificationResourceHealthMonitor(this);
		}
	}
}
