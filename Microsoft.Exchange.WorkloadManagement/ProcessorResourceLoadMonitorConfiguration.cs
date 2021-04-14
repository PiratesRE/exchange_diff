using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class ProcessorResourceLoadMonitorConfiguration : ResourceHealthMonitorConfiguration<ProcessorResourceLoadMonitorConfigurationSetting>
	{
		private ProcessorResourceLoadMonitorConfiguration()
		{
			if (this.overrideMetricValue < 0 || this.overrideMetricValue > 100)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int?>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] OverrideMetricValue '{0}' set in registry is not in the correct range (0-100). Ignore this setting.", this.overrideMetricValue);
				this.overrideMetricValue = null;
			}
			this.cpuAverageTimeWindow = this.ConfigSettings.DefaultCPUAverageTimeWindowInSeconds;
			this.healthyFairThreshold = this.ConfigSettings.DefaultHealthyFairThreshold;
			this.fairUnhealthyThreshold = this.ConfigSettings.DefaultFairUnhealthyThreshold;
			this.maxDelay = this.ConfigSettings.DefaultCPUMaxDelayInMilliseconds;
			using (RegistryKey registryKey = ResourceHealthMonitorConfiguration<ProcessorResourceLoadMonitorConfigurationSetting>.OpenConfigurationKey())
			{
				if (registryKey != null)
				{
					this.cpuAverageTimeWindow = (int)registryKey.GetValue(this.ConfigSettings.CPUAverageTimeWindowRegistryValueName, this.ConfigSettings.DefaultCPUAverageTimeWindowInSeconds);
					if (this.cpuAverageTimeWindow < 1 || this.cpuAverageTimeWindow > 60)
					{
						ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUAverageTimeWindow '{0}' set in registry is not in range [1, 60]. Using '{1}' instead.", this.cpuAverageTimeWindow, this.ConfigSettings.DefaultCPUAverageTimeWindowInSeconds);
						this.cpuAverageTimeWindow = this.ConfigSettings.DefaultCPUAverageTimeWindowInSeconds;
					}
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUAverageTimeWindow = '{0}'.", this.cpuAverageTimeWindow);
				}
			}
			IntAppSettingsEntry intAppSettingsEntry = new IntAppSettingsEntry(this.ConfigSettings.CPUHealthyFairThresholdConfigValueName, this.ConfigSettings.DefaultHealthyFairThreshold, ExTraceGlobals.ResourceHealthManagerTracer);
			this.healthyFairThreshold = intAppSettingsEntry.Value;
			if (this.healthyFairThreshold < 0 || this.healthyFairThreshold > 100)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUHealthyFairThreshold '{0}' set in App.Config file is not in range [0, 100]. Using '{1}' instead.", this.healthyFairThreshold, this.ConfigSettings.DefaultHealthyFairThreshold);
				this.healthyFairThreshold = this.ConfigSettings.DefaultHealthyFairThreshold;
			}
			IntAppSettingsEntry intAppSettingsEntry2 = new IntAppSettingsEntry(this.ConfigSettings.CPUFairUnhealthyThresholdConfigValueName, this.ConfigSettings.DefaultFairUnhealthyThreshold, ExTraceGlobals.ResourceHealthManagerTracer);
			this.fairUnhealthyThreshold = intAppSettingsEntry2.Value;
			if (this.fairUnhealthyThreshold < 0 || this.fairUnhealthyThreshold > 100)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUFairUnhealthyThreshold '{0}' set in App.Config file is not in range [0, 100]. Using '{1}' instead.", this.fairUnhealthyThreshold, this.ConfigSettings.DefaultFairUnhealthyThreshold);
				this.fairUnhealthyThreshold = this.ConfigSettings.DefaultFairUnhealthyThreshold;
			}
			if (this.healthyFairThreshold >= this.fairUnhealthyThreshold)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUHealthyFairThreshold '{0}' set in App.Config file is not smaller than CPUFairUnhealthyThreshold '{1}'. Using default values CPUHealthyFairThreshold='{2}' and CPUFairUnhealthyThreshold='{3}' instead.", new object[]
				{
					this.healthyFairThreshold,
					this.fairUnhealthyThreshold,
					this.ConfigSettings.DefaultHealthyFairThreshold,
					this.ConfigSettings.DefaultFairUnhealthyThreshold
				});
				this.healthyFairThreshold = this.ConfigSettings.DefaultHealthyFairThreshold;
				this.fairUnhealthyThreshold = this.ConfigSettings.DefaultFairUnhealthyThreshold;
			}
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUHealthyFairThreshold = '{0}'.", this.healthyFairThreshold);
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUFairUnhealthyThreshold = '{0}'.", this.fairUnhealthyThreshold);
			IntAppSettingsEntry intAppSettingsEntry3 = new IntAppSettingsEntry(this.ConfigSettings.CPUMaxDelayConfigValueName, this.ConfigSettings.DefaultCPUMaxDelayInMilliseconds, ExTraceGlobals.ResourceHealthManagerTracer);
			this.maxDelay = intAppSettingsEntry3.Value;
			if (this.maxDelay < 0 || this.maxDelay > 60000)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUMaxDelay '{0}' set in App.Config file is not in range [0, 60000]. Using '{1}' instead.", this.maxDelay, this.ConfigSettings.DefaultCPUMaxDelayInMilliseconds);
				this.maxDelay = this.ConfigSettings.DefaultCPUMaxDelayInMilliseconds;
			}
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProcessorResourceLoadMonitorConfiguration::ctor] CPUMaxDelay = '{0}'.", this.maxDelay);
		}

		public static TimeSpan RefreshInterval
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.Instance.refreshInterval;
			}
		}

		public static bool Enabled
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.Instance.enabled;
			}
		}

		public static int? OverrideMetricValue
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.Instance.overrideMetricValue;
			}
		}

		public static int CPUAverageTimeWindow
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.Instance.cpuAverageTimeWindow;
			}
		}

		public static int HealthyFairThreshold
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.Instance.healthyFairThreshold;
			}
		}

		public static int FairUnhealthyThreshold
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.Instance.fairUnhealthyThreshold;
			}
		}

		public static int MaxDelay
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.Instance.maxDelay;
			}
		}

		internal static ProcessorResourceLoadMonitorConfiguration Instance
		{
			get
			{
				if (ProcessorResourceLoadMonitorConfiguration.instance == null)
				{
					ProcessorResourceLoadMonitorConfiguration.instance = new ProcessorResourceLoadMonitorConfiguration();
				}
				return ProcessorResourceLoadMonitorConfiguration.instance;
			}
		}

		internal static void SetConfigurationValuesForTest(TimeSpan? refreshInterval, bool? enabled, int? overrideMetricValue, int? cpuAverageTimeWindow)
		{
			if (refreshInterval != null)
			{
				ProcessorResourceLoadMonitorConfiguration.Instance.refreshInterval = refreshInterval.Value;
			}
			if (enabled != null)
			{
				ProcessorResourceLoadMonitorConfiguration.Instance.enabled = enabled.Value;
			}
			if (overrideMetricValue != null)
			{
				ProcessorResourceLoadMonitorConfiguration.Instance.overrideMetricValue = new int?(overrideMetricValue.Value);
			}
			if (cpuAverageTimeWindow != null)
			{
				ProcessorResourceLoadMonitorConfiguration.Instance.cpuAverageTimeWindow = cpuAverageTimeWindow.Value;
			}
		}

		internal static void ReloadConfigurationValues()
		{
			ProcessorResourceLoadMonitorConfiguration.instance = null;
		}

		internal const int UnknownHealthMeasure = -1;

		internal const int UnhealthyHealthMeasure = 0;

		internal const int HealthyHealthMeasure = 100;

		private static ProcessorResourceLoadMonitorConfiguration instance;

		private readonly int healthyFairThreshold;

		private readonly int fairUnhealthyThreshold;

		private readonly int maxDelay;

		private int cpuAverageTimeWindow;
	}
}
