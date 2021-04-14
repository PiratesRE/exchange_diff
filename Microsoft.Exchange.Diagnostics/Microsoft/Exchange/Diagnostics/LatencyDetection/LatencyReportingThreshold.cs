using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LatencyReportingThreshold
	{
		internal LatencyReportingThreshold(LoggingType type, ushort needed, TimeSpan limit, TimeSpan value)
		{
			this.value = ((value >= LatencyReportingThreshold.minimumThresholdValue) ? value : LatencyReportingThreshold.minimumThresholdValue);
			this.loggingType = type;
			this.numberRequired = needed;
			this.requiredWithin = limit;
		}

		internal LatencyReportingThreshold(LoggingType type, TimeSpan threshold) : this(type, 1, LatencyReportingThreshold.defaultRequiredWithin, threshold)
		{
		}

		public static TimeSpan MinimumThresholdValue
		{
			get
			{
				return LatencyReportingThreshold.minimumThresholdValue;
			}
		}

		public ushort NumberRequired
		{
			get
			{
				return this.numberRequired;
			}
		}

		public TimeSpan TimeLimit
		{
			get
			{
				return this.requiredWithin;
			}
		}

		public TimeSpan Threshold
		{
			get
			{
				return this.value;
			}
		}

		public LoggingType LogType
		{
			get
			{
				return this.loggingType;
			}
		}

		private const ushort DefaultNumberRequired = 1;

		private static readonly TimeSpan defaultRequiredWithin = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan minimumThresholdValue = TimeSpan.FromMilliseconds(10.0);

		private ushort numberRequired;

		private TimeSpan requiredWithin;

		private LoggingType loggingType;

		private TimeSpan value;
	}
}
