using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class ProcessorResourceKey : ResourceKey
	{
		private ProcessorResourceKey() : base(ResourceMetricType.Processor, null)
		{
		}

		public static ProcessorResourceKey Local
		{
			get
			{
				return ProcessorResourceKey.local;
			}
		}

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new ProcessorResourceLoadMonitor(this);
		}

		private static ProcessorResourceKey local = new ProcessorResourceKey();
	}
}
