using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal class DummyResourceHealthMonitor : CacheableResourceHealthMonitor
	{
		internal DummyResourceHealthMonitor(ResourceKey key) : base(key)
		{
		}

		public override DateTime LastUpdateUtc
		{
			get
			{
				return TimeProvider.UtcNow;
			}
			protected internal set
			{
			}
		}

		public override ResourceHealthMonitorWrapper CreateWrapper()
		{
			return new DummyResourceHealthMonitorWrapper(this);
		}

		protected override int InternalMetricValue
		{
			get
			{
				return 0;
			}
		}

		public override ResourceLoad GetResourceLoad(WorkloadClassification classification, bool raw = false, object optionalData = null)
		{
			return ResourceLoad.Zero;
		}
	}
}
