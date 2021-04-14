using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.Data.Storage.ActiveManager.Performance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ActiveManagerPerformanceData
	{
		public static ActiveManagerPerformanceData.ProviderAndLogStrings[] Providers
		{
			get
			{
				if (ActiveManagerPerformanceData.providers == null)
				{
					ActiveManagerPerformanceData.providers = new ActiveManagerPerformanceData.ProviderAndLogStrings[]
					{
						new ActiveManagerPerformanceData.ProviderAndLogStrings(ActiveManagerPerformanceData.CalculatePreferredHomeServerDataProvider)
					};
				}
				return ActiveManagerPerformanceData.providers;
			}
		}

		public static PerformanceDataProvider CalculatePreferredHomeServerDataProvider
		{
			get
			{
				if (ActiveManagerPerformanceData.calculatePreferredHomeServerDataProvider == null)
				{
					ActiveManagerPerformanceData.calculatePreferredHomeServerDataProvider = new PerformanceDataProvider("ActiveManager.CalculatePreferredHomeServer");
				}
				return ActiveManagerPerformanceData.calculatePreferredHomeServerDataProvider;
			}
		}

		[ThreadStatic]
		private static ActiveManagerPerformanceData.ProviderAndLogStrings[] providers;

		[ThreadStatic]
		private static PerformanceDataProvider calculatePreferredHomeServerDataProvider;

		public class ProviderAndLogStrings
		{
			public ProviderAndLogStrings(IPerformanceDataProvider provider)
			{
				this.provider = provider;
				this.logCount = string.Format("{0}.Count", provider.Name);
				this.logLatency = string.Format("{0}.Latency", provider.Name);
			}

			public IPerformanceDataProvider Provider
			{
				get
				{
					return this.provider;
				}
			}

			public string LogCount
			{
				get
				{
					return this.logCount;
				}
			}

			public string LogLatency
			{
				get
				{
					return this.logLatency;
				}
			}

			private readonly IPerformanceDataProvider provider;

			private readonly string logCount;

			private readonly string logLatency;
		}
	}
}
