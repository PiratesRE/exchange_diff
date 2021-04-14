using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class ReplayLagStatus
	{
		public EnhancedTimeSpan ConfiguredLagTime { get; private set; }

		public EnhancedTimeSpan ActualLagTime { get; private set; }

		public int Percentage { get; private set; }

		public string DisabledReason { get; private set; }

		public bool Enabled { get; private set; }

		public ReplayLagPlayDownReason PlayDownReason { get; private set; }

		internal ReplayLagStatus(bool isLagInEffect, EnhancedTimeSpan configuredLagTime, EnhancedTimeSpan actualLagTime, int lagPercentage, ReplayLagPlayDownReason playDownReason, string disabledReason)
		{
			this.Enabled = isLagInEffect;
			this.ConfiguredLagTime = configuredLagTime;
			this.ActualLagTime = actualLagTime;
			this.Percentage = lagPercentage;
			this.PlayDownReason = playDownReason;
			this.DisabledReason = disabledReason;
		}

		public override string ToString()
		{
			return string.Format("{3}:{0}; {7}:{6}; {9}:{8}; {4}:{1}; {5}:{2}", new object[]
			{
				this.Enabled,
				this.ConvertTimeSpanToString(this.ConfiguredLagTime),
				this.ConvertTimeSpanToString(this.ActualLagTime),
				Strings.ReplayLagStatusLabelEnabled,
				Strings.ReplayLagStatusLabelConfigured,
				Strings.ReplayLagStatusLabelActual,
				this.PlayDownReason,
				Strings.ReplayLagStatusLabelPlayDownReason,
				this.Percentage,
				Strings.ReplayLagStatusLabelPercentage
			});
		}

		private string ConvertTimeSpanToString(EnhancedTimeSpan timeSpan)
		{
			return DateTimeHelper.ConvertTimeSpanToShortString(timeSpan);
		}

		private const string ToStringFormatStr = "{3}:{0}; {7}:{6}; {9}:{8}; {4}:{1}; {5}:{2}";
	}
}
