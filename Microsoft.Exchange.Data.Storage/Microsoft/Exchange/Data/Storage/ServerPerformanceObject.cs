using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ServerPerformanceObject
	{
		public ServerPerformanceObject(Fqdn identity)
		{
			string key = identity.ToString().ToLower();
			lock (ServerPerformanceObject.latencyDetectionContextFactoriesLock)
			{
				if (!ServerPerformanceObject.latencyDetectionContextFactories.TryGetValue(key, out this.latencyDetectionContextFactory))
				{
					this.latencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory(identity);
					ServerPerformanceObject.latencyDetectionContextFactories[key] = this.latencyDetectionContextFactory;
				}
			}
		}

		public TimeSpan? LastRpcLatency
		{
			get
			{
				return this.lastRpcLatency;
			}
		}

		public TimeSpan AverageRpcLatency
		{
			get
			{
				return new TimeSpan((long)this.averageRpcLatency);
			}
		}

		public void Start()
		{
			if (this.latencyDetectionContext != null)
			{
				throw new InvalidOperationException("Start has already been invoked.");
			}
			this.lastRpcLatency = null;
			this.latencyDetectionContext = this.latencyDetectionContextFactory.CreateContext(ContextOptions.DoNotMeasureTime | ContextOptions.DoNotCreateReport, ServerPerformanceObject.Version, this, new IPerformanceDataProvider[]
			{
				RpcDataProvider.Instance
			});
		}

		public void StopAndCollectPerformanceData()
		{
			if (this.latencyDetectionContext == null)
			{
				throw new InvalidOperationException("Must call Start before Stop for collecting performance data.");
			}
			TaskPerformanceData[] array = this.latencyDetectionContext.StopAndFinalizeCollection();
			this.latencyDetectionContext = null;
			if (array == null || array.Length <= 0)
			{
				return;
			}
			this.lastRpcLatency = new TimeSpan?(array[0].Difference.Latency);
			if (this.lastRpcLatency != null)
			{
				double currentValue = (double)this.lastRpcLatency.Value.Ticks;
				this.averageRpcLatency = ServerPerformanceObject.ComputeAveragedPerformanceValue(this.averageRpcLatency, currentValue, 1024);
			}
		}

		private static double ComputeAveragedPerformanceValue(double averagedValue, double currentValue, int sampleSize)
		{
			return (averagedValue * (double)sampleSize - averagedValue + currentValue) / (double)sampleSize;
		}

		public const int LatencySamples = 1024;

		private static readonly Dictionary<string, LatencyDetectionContextFactory> latencyDetectionContextFactories = new Dictionary<string, LatencyDetectionContextFactory>();

		private static readonly object latencyDetectionContextFactoriesLock = new object();

		private static readonly string Version = typeof(ServerPerformanceObject).GetApplicationVersion();

		private readonly LatencyDetectionContextFactory latencyDetectionContextFactory;

		private LatencyDetectionContext latencyDetectionContext;

		private double averageRpcLatency;

		private TimeSpan? lastRpcLatency = null;
	}
}
