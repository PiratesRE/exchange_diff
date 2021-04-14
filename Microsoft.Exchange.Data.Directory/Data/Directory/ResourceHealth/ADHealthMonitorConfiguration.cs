using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal sealed class ADHealthMonitorConfiguration : ResourceHealthMonitorConfiguration<ADHealthMonitorConfigurationSetting>
	{
		internal static ADHealthMonitorConfiguration Instance
		{
			get
			{
				if (ADHealthMonitorConfiguration.instance == null)
				{
					ADHealthMonitorConfiguration.instance = new ADHealthMonitorConfiguration();
				}
				return ADHealthMonitorConfiguration.instance;
			}
		}

		private ADHealthMonitorConfiguration()
		{
			this.enabled = (this.enabled && Globals.IsMicrosoftHostedOnly);
			ADHealthMonitorConfiguration.Tracer.TraceDebug<bool>((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] Enabled={0}.", this.enabled);
			if (this.refreshInterval < this.ConfigSettings.DefaultRefreshInterval)
			{
				ADHealthMonitorConfiguration.Tracer.TraceError<TimeSpan, TimeSpan>((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] Refresh interval '{0}' set in registry is less than '{1}'. Using '{1}' instead.", this.refreshInterval, this.ConfigSettings.DefaultRefreshInterval);
				this.refreshInterval = this.ConfigSettings.DefaultRefreshInterval;
			}
			using (RegistryKey registryKey = ResourceHealthMonitorConfiguration<ADHealthMonitorConfigurationSetting>.OpenConfigurationKey())
			{
				if (registryKey != null)
				{
					this.overrideHealthMeasure = (registryKey.GetValue(this.ConfigSettings.OverrideHealthMeasureRegistryValueName) as int?);
					if (this.overrideHealthMeasure > 100 || this.overrideHealthMeasure < -1)
					{
						ADHealthMonitorConfiguration.Tracer.TraceError<int?>((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] OverrideHealthMeasure '{0}' set in registry is not in range [-1, 100]. Using '<null>' instead.", this.overrideHealthMeasure);
					}
					ADHealthMonitorConfiguration.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] OverrideHealthMeasure={0}.", (this.overrideHealthMeasure != null) ? this.overrideHealthMeasure.Value.ToString() : "<null>");
					this.healthyCutoff = ResourceHealthMonitorConfiguration<ADHealthMonitorConfigurationSetting>.ReadTimeSpan(registryKey, this.ConfigSettings.HealthyCutoffRegistryValueName, this.ConfigSettings.HealthyCutoffDefault);
					if (this.healthyCutoff < this.refreshInterval)
					{
						ADHealthMonitorConfiguration.Tracer.TraceError<TimeSpan, TimeSpan>((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] Healthy cutoff '{0}' set in registry is less than refresh interval '{1}'. Using '{1}' instead.", this.healthyCutoff, this.refreshInterval);
						this.healthyCutoff = this.refreshInterval;
					}
					ADHealthMonitorConfiguration.Tracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] HealthyCutoff = '{0}'.", this.healthyCutoff);
					this.fairCutoff = ResourceHealthMonitorConfiguration<ADHealthMonitorConfigurationSetting>.ReadTimeSpan(registryKey, this.ConfigSettings.FairCutoffRegistryValueName, this.ConfigSettings.FairCutoffDefault);
					if (this.fairCutoff < this.healthyCutoff + this.refreshInterval)
					{
						ADHealthMonitorConfiguration.Tracer.TraceError((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] Fair cutoff '{0}' set in registry is less than healthy cutoff '{1}' plus refresh interval '{2}'. Using '{3}' instead.", new object[]
						{
							this.fairCutoff,
							this.healthyCutoff,
							this.refreshInterval,
							this.healthyCutoff + this.refreshInterval
						});
						this.fairCutoff = this.healthyCutoff + this.refreshInterval;
					}
					ADHealthMonitorConfiguration.Tracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "[ADHealthMonitorConfiguration::ctor] FairCutoff = '{0}'.", this.fairCutoff);
				}
			}
		}

		public static int? OverrideHealthMeasure
		{
			get
			{
				return ADHealthMonitorConfiguration.Instance.overrideHealthMeasure;
			}
		}

		public static TimeSpan HealthyCutoff
		{
			get
			{
				return ADHealthMonitorConfiguration.Instance.healthyCutoff;
			}
		}

		public static TimeSpan FairCutoff
		{
			get
			{
				return ADHealthMonitorConfiguration.Instance.fairCutoff;
			}
		}

		public static TimeSpan RefreshInterval
		{
			get
			{
				return ADHealthMonitorConfiguration.Instance.refreshInterval;
			}
		}

		public static bool Enabled
		{
			get
			{
				return ADHealthMonitorConfiguration.Instance.enabled;
			}
		}

		private readonly int? overrideHealthMeasure = null;

		private readonly TimeSpan healthyCutoff;

		private readonly TimeSpan fairCutoff;

		private static readonly Trace Tracer = ExTraceGlobals.ResourceHealthManagerTracer;

		private static ADHealthMonitorConfiguration instance;
	}
}
