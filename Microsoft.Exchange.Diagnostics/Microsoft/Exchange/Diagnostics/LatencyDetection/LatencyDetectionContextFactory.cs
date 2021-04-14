using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LatencyDetectionContextFactory
	{
		private LatencyDetectionContextFactory(LatencyDetectionLocation factoryLocation)
		{
			this.location = factoryLocation;
		}

		public string LocationIdentity
		{
			get
			{
				return this.location.Identity;
			}
		}

		public static LatencyDetectionContextFactory CreateFactory(string identity, TimeSpan minimumThreshold, TimeSpan defaultThreshold)
		{
			LatencyDetectionLocation factoryLocation = new LatencyDetectionLocation(LatencyDetectionContextFactory.thresholdInitializer, identity, minimumThreshold, defaultThreshold);
			return new LatencyDetectionContextFactory(factoryLocation);
		}

		public static LatencyDetectionContextFactory CreateFactory(string identity)
		{
			return LatencyDetectionContextFactory.CreateFactory(identity, LatencyReportingThreshold.MinimumThresholdValue, TimeSpan.MaxValue);
		}

		public LatencyDetectionContext CreateContext(string version)
		{
			return this.CreateContext(version, this.location.Identity, new IPerformanceDataProvider[0]);
		}

		public LatencyDetectionContext CreateContext(string version, object hash, params IPerformanceDataProvider[] providers)
		{
			return this.CreateContext(ContextOptions.Default, version, hash, providers);
		}

		public LatencyReportingThreshold GetThreshold(LoggingType type)
		{
			return this.location.GetThreshold(type);
		}

		public LatencyDetectionContext CreateContext(ContextOptions contextOptions, string version, object hash, params IPerformanceDataProvider[] providers)
		{
			LatencyDetectionContext.ValidateBinningParameters(this.location, version, hash);
			return new LatencyDetectionContext(this.location, contextOptions, version, hash, providers);
		}

		private static readonly IThresholdInitializer thresholdInitializer = PerformanceReportingOptions.Instance;

		private readonly LatencyDetectionLocation location;
	}
}
