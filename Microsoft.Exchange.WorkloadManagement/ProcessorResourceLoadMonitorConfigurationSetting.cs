using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class ProcessorResourceLoadMonitorConfigurationSetting : ResourceHealthMonitorConfigurationSetting
	{
		internal override string DisabledRegistryValueName
		{
			get
			{
				return "DisableCPUHealthCollection";
			}
		}

		internal override string RefreshIntervalRegistryValueName
		{
			get
			{
				return "CPUHealthRefreshInterval";
			}
		}

		internal override string OverrideMetricValueRegistryValueName
		{
			get
			{
				return "CPUMetricValue";
			}
		}

		internal string CPUAverageTimeWindowRegistryValueName
		{
			get
			{
				return "CPUAverageTimeWindowInSeconds";
			}
		}

		internal string CPUHealthyFairThresholdConfigValueName
		{
			get
			{
				return "CPUHealthyFairThreshold";
			}
		}

		internal string CPUFairUnhealthyThresholdConfigValueName
		{
			get
			{
				return "CPUFairUnhealthyThreshold";
			}
		}

		internal string CPUMaxDelayConfigValueName
		{
			get
			{
				return "CPUMaxDelayInMilliseconds";
			}
		}

		internal override TimeSpan DefaultRefreshInterval
		{
			get
			{
				return this.refreshIntervalDefault;
			}
		}

		internal int DefaultCPUAverageTimeWindowInSeconds
		{
			get
			{
				return 15;
			}
		}

		internal int DefaultHealthyFairThreshold
		{
			get
			{
				return 70;
			}
		}

		internal int DefaultFairUnhealthyThreshold
		{
			get
			{
				return 80;
			}
		}

		internal int DefaultCPUMaxDelayInMilliseconds
		{
			get
			{
				return 15000;
			}
		}

		private const string DisabledValueName = "DisableCPUHealthCollection";

		private const string RefreshIntervalValueName = "CPUHealthRefreshInterval";

		private const string OverrideMetricValueName = "CPUMetricValue";

		private const string CPUAverageTimeWindowValueName = "CPUAverageTimeWindowInSeconds";

		private const string CPUHealthyFairThresholdValueName = "CPUHealthyFairThreshold";

		private const string CPUFairUnhealthyThresholdValueName = "CPUFairUnhealthyThreshold";

		private const string CPUMaxDelayValueName = "CPUMaxDelayInMilliseconds";

		private readonly TimeSpan refreshIntervalDefault = TimeSpan.FromSeconds(1.0);
	}
}
