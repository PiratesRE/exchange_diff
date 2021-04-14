using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal sealed class ADResourceKey : ResourceKey
	{
		private ADResourceKey() : base(ResourceMetricType.ActiveDirectoryReplicationLatency, null)
		{
		}

		public static ADResourceKey Key
		{
			get
			{
				return ADResourceKey.key;
			}
		}

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new ADResourceHealthMonitor(this);
		}

		private static ADResourceKey key = new ADResourceKey();
	}
}
