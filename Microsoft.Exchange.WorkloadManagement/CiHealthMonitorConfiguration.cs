using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class CiHealthMonitorConfiguration : ResourceHealthMonitorConfiguration<CiHealthMonitorConfigurationSetting>
	{
		private CiHealthMonitorConfiguration()
		{
			this.numberOfHealthyCopiesRequired = this.ConfigSettings.DefaultNumberOfHealthyCopiesRequired;
			this.rpcTimeout = this.ConfigSettings.DefaultRpcTimeout;
			this.failedCatalogStatusThreshold = this.ConfigSettings.DefaultFailedCatalogStatusThreshold;
			this.mdbCopyUpdateInterval = this.ConfigSettings.DefaultMdbCopyUpdateInterval;
			this.mdbCopyUpdateDelay = this.ConfigSettings.DefaultMdbCopyUpdateDelay;
			using (RegistryKey registryKey = ResourceHealthMonitorConfiguration<CiHealthMonitorConfigurationSetting>.OpenConfigurationKey())
			{
				if (registryKey != null)
				{
					this.numberOfHealthyCopiesRequired = (int)registryKey.GetValue(this.ConfigSettings.NumberOfHealthyCopiesRequiredRegistryValueName, this.ConfigSettings.DefaultNumberOfHealthyCopiesRequired);
					if (this.numberOfHealthyCopiesRequired < 1 || this.numberOfHealthyCopiesRequired > 4)
					{
						ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, int>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] NumberOfHealthyCopiesRequired '{0}' set in registry is not in range [1, 4]. Using '{1}' instead.", this.numberOfHealthyCopiesRequired, this.ConfigSettings.DefaultNumberOfHealthyCopiesRequired);
						this.numberOfHealthyCopiesRequired = this.ConfigSettings.DefaultNumberOfHealthyCopiesRequired;
					}
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] NumberOfHealthyCopiesRequired = '{0}'.", this.numberOfHealthyCopiesRequired);
					int num = (int)registryKey.GetValue(this.ConfigSettings.RpcTimeoutRegistryValueName, (int)this.ConfigSettings.DefaultRpcTimeout.TotalSeconds);
					if (num <= 0 || num > 60)
					{
						ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, double>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] RpcTimeout '{0}' set in registry is not in range [1, 60]. Using '{1}' instead.", num, this.ConfigSettings.DefaultRpcTimeout.TotalSeconds);
						num = (int)this.ConfigSettings.DefaultRpcTimeout.TotalSeconds;
					}
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] RpcTimeout = '{0}'.", num);
					this.rpcTimeout = TimeSpan.FromSeconds((double)num);
					int num2 = (int)registryKey.GetValue(this.ConfigSettings.MdbCopyUpdateDelayRegistryValueName, (int)this.ConfigSettings.DefaultMdbCopyUpdateDelay.TotalSeconds);
					if (num2 <= 0 || num2 > 30)
					{
						ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, double>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] MDB refresh delay '{0}' set in registry is not in range [1, 30]. Using '{1}' instead.", num2, this.ConfigSettings.DefaultMdbCopyUpdateDelay.TotalSeconds);
						num2 = (int)this.ConfigSettings.DefaultMdbCopyUpdateDelay.TotalSeconds;
					}
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] MDB refresh delay = '{0}'.", num2);
					this.mdbCopyUpdateDelay = TimeSpan.FromSeconds((double)num2);
					int num3 = (int)registryKey.GetValue(this.ConfigSettings.MdbCopyUpdateIntervalRegistryValueName, (int)this.ConfigSettings.DefaultMdbCopyUpdateInterval.TotalSeconds);
					if (num3 <= 60 || num3 > 3600)
					{
						ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int, double>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] MDB refresh interval '{0}' set in registry is not in range [60, 3600]. Using '{1}' instead.", num3, this.ConfigSettings.DefaultMdbCopyUpdateInterval.TotalSeconds);
						num3 = (int)this.ConfigSettings.DefaultMdbCopyUpdateInterval.TotalSeconds;
					}
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] MDB refresh interval = '{0}'.", num3);
					this.mdbCopyUpdateInterval = TimeSpan.FromSeconds((double)num3);
					this.failedCatalogStatusThreshold = (int)registryKey.GetValue(this.ConfigSettings.FailedCatalogStatusThresholdRegistryValueName, this.ConfigSettings.DefaultFailedCatalogStatusThreshold);
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<int>((long)this.GetHashCode(), "[CiHealthMonitorConfiguration::ctor] FailedCatalogStatusThreshold = '{0}'.", this.failedCatalogStatusThreshold);
				}
			}
		}

		public static TimeSpan RefreshInterval
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.refreshInterval;
			}
			set
			{
				CiHealthMonitorConfiguration.Instance.refreshInterval = value;
			}
		}

		public static bool Enabled
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.enabled;
			}
		}

		public static int? OverrideMetricValue
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.overrideMetricValue;
			}
		}

		public static int NumberOfHealthyCopiesRequired
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.numberOfHealthyCopiesRequired;
			}
		}

		public static int FailedCatalogStatusThreshold
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.failedCatalogStatusThreshold;
			}
		}

		public static TimeSpan RpcTimeout
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.rpcTimeout;
			}
		}

		public static TimeSpan MdbCopyUpdateInterval
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.mdbCopyUpdateInterval;
			}
		}

		public static TimeSpan MdbCopyUpdateDelay
		{
			get
			{
				return CiHealthMonitorConfiguration.Instance.mdbCopyUpdateDelay;
			}
		}

		internal static CiHealthMonitorConfiguration Instance
		{
			get
			{
				if (CiHealthMonitorConfiguration.instance == null)
				{
					CiHealthMonitorConfiguration.instance = new CiHealthMonitorConfiguration();
				}
				return CiHealthMonitorConfiguration.instance;
			}
		}

		private static CiHealthMonitorConfiguration instance;

		private readonly int numberOfHealthyCopiesRequired;

		private readonly int failedCatalogStatusThreshold;

		private readonly TimeSpan rpcTimeout;

		private readonly TimeSpan mdbCopyUpdateInterval;

		private readonly TimeSpan mdbCopyUpdateDelay;
	}
}
