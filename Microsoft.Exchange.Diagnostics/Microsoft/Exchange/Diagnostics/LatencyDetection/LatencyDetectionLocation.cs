using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LatencyDetectionLocation
	{
		internal LatencyDetectionLocation(IThresholdInitializer thresholdInitializer, string identity, TimeSpan minimumThreshold, TimeSpan defaultThreshold)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentException("Shouldn't be null or empty.", "identity");
			}
			if (minimumThreshold < LatencyReportingThreshold.MinimumThresholdValue)
			{
				throw new ArgumentException("May not be less than global minimum threshold.", "minimumThreshold");
			}
			if (defaultThreshold < minimumThreshold)
			{
				throw new ArgumentException("May not be less than minimum threshold.", "defaultThreshold");
			}
			this.identity = identity;
			this.minimumThreshold = minimumThreshold;
			this.defaultThreshold = defaultThreshold;
			for (int i = 0; i < LatencyDetectionLocation.ArrayLength; i++)
			{
				this.backlogs[i] = new BackLog(this.thresholds[i]);
				thresholdInitializer.SetThresholdFromConfiguration(this, (LoggingType)i);
			}
			LatencyReportingThresholdContainer.Instance.ValidateLocation(this);
		}

		internal string Identity
		{
			get
			{
				return this.identity;
			}
		}

		internal TimeSpan MinimumThreshold
		{
			get
			{
				return this.minimumThreshold;
			}
		}

		internal TimeSpan DefaultThreshold
		{
			get
			{
				return this.defaultThreshold;
			}
		}

		internal LatencyReportingThreshold GetThreshold(LoggingType type)
		{
			return this.thresholds[(int)type];
		}

		internal LatencyReportingThreshold GetThreshold(int type)
		{
			LatencyDetectionLocation.CheckLoggingTypeIndex(type);
			return this.thresholds[type];
		}

		internal BackLog GetBackLog(LoggingType type)
		{
			return this.backlogs[(int)type];
		}

		internal BackLog GetBackLog(int type)
		{
			LatencyDetectionLocation.CheckLoggingTypeIndex(type);
			return this.backlogs[type];
		}

		internal void ClearBackLogs()
		{
			for (int i = 0; i < LatencyDetectionLocation.ArrayLength; i++)
			{
				this.backlogs[i].Clear();
			}
		}

		internal void ClearThresholds()
		{
			foreach (object obj in Enum.GetValues(typeof(LoggingType)))
			{
				LoggingType logType = (LoggingType)obj;
				this.SetThreshold(logType, TimeSpan.MaxValue);
			}
		}

		internal LatencyReportingThreshold SetThreshold(LoggingType logType, TimeSpan threshold)
		{
			LatencyReportingThreshold latencyReportingThreshold = new LatencyReportingThreshold(logType, threshold);
			this.thresholds[(int)logType] = latencyReportingThreshold;
			this.backlogs[(int)logType].ChangeThresholdAndClear(latencyReportingThreshold);
			return latencyReportingThreshold;
		}

		private static void CheckLoggingTypeIndex(int type)
		{
			if (type < 0 || type >= LatencyDetectionLocation.ArrayLength)
			{
				throw new ArgumentOutOfRangeException("type", type, LatencyDetectionLocation.TypeLimits);
			}
		}

		internal static readonly int ArrayLength = Enum.GetValues(typeof(LoggingType)).Length;

		private static readonly string TypeLimits = "Needs to be 0 to " + (LatencyDetectionLocation.ArrayLength - 1) + ".";

		private readonly string identity;

		private readonly TimeSpan minimumThreshold;

		private readonly TimeSpan defaultThreshold;

		private readonly LatencyReportingThreshold[] thresholds = new LatencyReportingThreshold[LatencyDetectionLocation.ArrayLength];

		private readonly BackLog[] backlogs = new BackLog[LatencyDetectionLocation.ArrayLength];
	}
}
