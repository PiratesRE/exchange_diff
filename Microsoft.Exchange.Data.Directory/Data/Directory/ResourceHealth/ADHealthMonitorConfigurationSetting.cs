using System;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal sealed class ADHealthMonitorConfigurationSetting : ResourceHealthMonitorConfigurationSetting
	{
		internal override string DisabledRegistryValueName
		{
			get
			{
				return "DisableADHealthCollection";
			}
		}

		internal override string RefreshIntervalRegistryValueName
		{
			get
			{
				return "ADHealthRefreshInterval";
			}
		}

		internal override string OverrideMetricValueRegistryValueName
		{
			get
			{
				return "ADMetricValue";
			}
		}

		internal string OverrideHealthMeasureRegistryValueName
		{
			get
			{
				return "ADHealthMeasure";
			}
		}

		internal string HealthyCutoffRegistryValueName
		{
			get
			{
				return "ADHealthHealthyCutoff";
			}
		}

		internal string FairCutoffRegistryValueName
		{
			get
			{
				return "ADHealthFairCutoff";
			}
		}

		internal override TimeSpan DefaultRefreshInterval
		{
			get
			{
				return ADHealthMonitorConfigurationSetting.refreshIntervalDefault;
			}
		}

		internal TimeSpan HealthyCutoffDefault
		{
			get
			{
				return ADHealthMonitorConfigurationSetting.healthyCutoffDefault;
			}
		}

		internal TimeSpan FairCutoffDefault
		{
			get
			{
				return ADHealthMonitorConfigurationSetting.fairCutoffDefault;
			}
		}

		private const string DisabledValueName = "DisableADHealthCollection";

		private const string RefreshIntervalValueName = "ADHealthRefreshInterval";

		private const string OverrideMetricValueName = "ADMetricValue";

		private const string OverrideHealthMeasureValueName = "ADHealthMeasure";

		private const string HealthyCutoffValueName = "ADHealthHealthyCutoff";

		private const string FairCutoffValueName = "ADHealthFairCutoff";

		private static readonly TimeSpan refreshIntervalDefault = TimeSpan.FromMinutes(3.0);

		private static readonly TimeSpan healthyCutoffDefault = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan fairCutoffDefault = TimeSpan.FromMinutes(45.0);
	}
}
