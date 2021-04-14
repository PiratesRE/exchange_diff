using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class CiHealthMonitorConfigurationSetting : ResourceHealthMonitorConfigurationSetting
	{
		internal override string DisabledRegistryValueName
		{
			get
			{
				return "DisableCiHealthCollection";
			}
		}

		internal override string RefreshIntervalRegistryValueName
		{
			get
			{
				return "CiHealthRefreshInterval";
			}
		}

		internal override string OverrideMetricValueRegistryValueName
		{
			get
			{
				return "CiMetricValue";
			}
		}

		internal string NumberOfHealthyCopiesRequiredRegistryValueName
		{
			get
			{
				return "CiNumberOfHealthyCopiesRequired";
			}
		}

		internal string FailedCatalogStatusThresholdRegistryValueName
		{
			get
			{
				return "CiFailedCatalogStatusThreshold";
			}
		}

		internal string RpcTimeoutRegistryValueName
		{
			get
			{
				return "CiRpcTimeoutInterval";
			}
		}

		internal string MdbCopyUpdateIntervalRegistryValueName
		{
			get
			{
				return "CiMdbCopyUpdateInterval";
			}
		}

		internal string MdbCopyUpdateDelayRegistryValueName
		{
			get
			{
				return "CiMdbCopyUpdateDelay";
			}
		}

		internal override TimeSpan DefaultRefreshInterval
		{
			get
			{
				return TimeSpan.FromSeconds(10.0);
			}
		}

		internal TimeSpan DefaultRpcTimeout
		{
			get
			{
				return TimeSpan.FromSeconds(10.0);
			}
		}

		internal TimeSpan DefaultMdbCopyUpdateInterval
		{
			get
			{
				return TimeSpan.FromMinutes(15.0);
			}
		}

		internal TimeSpan DefaultMdbCopyUpdateDelay
		{
			get
			{
				return TimeSpan.FromSeconds(5.0);
			}
		}

		internal int DefaultNumberOfHealthyCopiesRequired
		{
			get
			{
				return 2;
			}
		}

		internal int DefaultFailedCatalogStatusThreshold
		{
			get
			{
				return 6;
			}
		}

		private const string DisabledValueName = "DisableCiHealthCollection";

		private const string RefreshIntervalValueName = "CiHealthRefreshInterval";

		private const string OverrideMetricValueName = "CiMetricValue";

		private const string NumberOfHealthyCopiesRequiredValueName = "CiNumberOfHealthyCopiesRequired";

		private const string FailedCatalogStatusThresholdValueName = "CiFailedCatalogStatusThreshold";

		private const string RpcTimeoutValueName = "CiRpcTimeoutInterval";

		private const string MdbCopyUpdateIntervalValueName = "CiMdbCopyUpdateInterval";

		private const string MdbCopyUpdateDelayValueName = "CiMdbCopyUpdateDelay";
	}
}
