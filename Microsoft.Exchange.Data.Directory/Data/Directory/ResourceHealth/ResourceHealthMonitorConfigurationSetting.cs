using System;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal abstract class ResourceHealthMonitorConfigurationSetting
	{
		internal abstract string DisabledRegistryValueName { get; }

		internal abstract string RefreshIntervalRegistryValueName { get; }

		internal abstract string OverrideMetricValueRegistryValueName { get; }

		internal abstract TimeSpan DefaultRefreshInterval { get; }
	}
}
