using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Optics
{
	internal class PerformanceCounterAccessorRegistry : IPerformanceCounterAccessorRegistry
	{
		private PerformanceCounterAccessorRegistry()
		{
		}

		public static PerformanceCounterAccessorRegistry Instance
		{
			get
			{
				return PerformanceCounterAccessorRegistry.instance;
			}
		}

		public IPerformanceCounterAccessor GetOrAddPerformanceCounterAccessor(string type)
		{
			return this.accessorLookupTable.GetOrAdd(type, new PerformanceCounterAccessor(type));
		}

		internal IEnumerable<PerformanceCounterAccessor> GetAllRegisteredAccessors()
		{
			return this.accessorLookupTable.Values;
		}

		private static PerformanceCounterAccessorRegistry instance = new PerformanceCounterAccessorRegistry();

		private ConcurrentDictionary<string, PerformanceCounterAccessor> accessorLookupTable = new ConcurrentDictionary<string, PerformanceCounterAccessor>();
	}
}
