using System;
using System.Linq;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal static class HighAvailabilityConstants
	{
		public static int ReqdDataProtectionInfrastructureDetection
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureDetection);
			}
		}

		public static int ReplayRestartWindow
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ReplayRestartWindow);
			}
		}

		public static int ReqdDataProtectionInfrastructureBugcheck
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureBugcheck);
			}
		}

		public static int ReqdDataProtectionInfrastructureEscalate2
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureEscalate2);
			}
		}

		public static int NonHealthThreatingDataProtectionInfraDetection
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ReplayRestartWindow);
			}
		}

		public static int TransientNonHealthThreatingDataProtectionInfraDetection
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.TransientNonHealthThreatingDataProtectionInfraDetection);
			}
		}

		public static int NonHealthThreatingDataProtectionInfraRepair1
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.TransientNonHealthThreatingDataProtectionInfraDetection);
			}
		}

		public static int NonHealthThreatingDataProtectionInfraRepair2
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.NonHealthThreatingDataProtectionInfraRepair2);
			}
		}

		public static int ReqdDataProtectionInfrastructureEscalate1
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureBugcheck);
			}
		}

		public static int ReqdDataProtectionInfrastructureRecoveryFailure
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureBugcheck);
			}
		}

		public static int TransientSuppressedLoadDetectionWindow
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.TransientSuppressedLoadDetectionWindow);
			}
		}

		public static int AdministrativelyDerivedFailureDetection
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.AdministrativelyDerivedFailureDetection);
			}
		}

		public static int EstimatedReseedTime
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.AdministrativelyDerivedFailureDetection);
			}
		}

		public static int ServerInMaintenanceModeTurnaroundTime
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.ServerInMaintenanceModeTurnaroundTime);
			}
		}

		public static int ProbeFailureDuration
		{
			get
			{
				return HighAvailabilityConstants.GetTimings(HighAvailabilityConstants.Timings.TransientNonHealthThreatingDataProtectionInfraDetection);
			}
		}

		public static bool DisableResponders
		{
			get
			{
				int value = HighAvailabilityUtility.RegReader.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters", "BigRedButton", 0);
				return value != 0;
			}
		}

		private static int GetTimings(HighAvailabilityConstants.Timings timing)
		{
			int result = (int)timing;
			if (HighAvailabilityConstants.overrideAllowedTimings.Contains(timing))
			{
				int value = HighAvailabilityUtility.RegReader.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters", timing.ToString(), 0);
				if (value > 0)
				{
					result = value;
				}
			}
			return result;
		}

		public const string ParameterRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters";

		public const string StateRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States";

		public const string DbCopyStateRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States\\DbCopyStates";

		public const string ServerComponentStateRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States\\ServerComponentStates";

		public const char StrikeHistoryFieldSeperator = '|';

		public const char StrikeHistoryEntrySeperator = ';';

		public const string StrikeHistoryRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\StrikeHistory";

		public const string DiskLatencyWatermarkValueName = "DiskLatencyWatermark";

		public const string OneCopyMonitorLastRunValueName = "OneCopyMonitorLastRun";

		public const string OneCopyMonitorStaleAlertValueName = "OneCopyMonitorStaleAlertInMins";

		public const string WorkitemEnrollmentLog = "WorkItemEnrollmentLogPath";

		public const string AdCacheExpirationValueName = "AdCacheExpirationInSeconds";

		public const string RpcCacheExpirationValueName = "RpcCacheExpirationInSeconds";

		public const string RegCacheExpirationValueName = "RegCacheExpirationInSeconds";

		public const string EscalationTeam = "High Availability";

		public const string EseEscalationTeam = "ESE";

		public const string ReplServiceName = "MSExchangeRepl";

		public const string DagMgmtServiceName = "MSExchangeDagMgmt";

		public const string StoreServiceName = "MSExchangeIS";

		public const string NetworkServiceName = "Network";

		public const int ProbeMaxRetry = 3;

		public const int DefaultMonitoringInterval = 300;

		private const string ResponderDisableMasterSwitch = "BigRedButton";

		public static readonly string ServiceName = ExchangeComponent.DataProtection.Name;

		public static readonly string ControllerServiceName = ExchangeComponent.DiskController.Name;

		public static readonly string ClusteringServiceName = ExchangeComponent.Clustering.Name;

		public static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly HighAvailabilityConstants.Timings[] overrideAllowedTimings = new HighAvailabilityConstants.Timings[]
		{
			HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureBugcheck,
			HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureEscalate2,
			HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureBugcheck,
			HighAvailabilityConstants.Timings.ReqdDataProtectionInfrastructureBugcheck,
			HighAvailabilityConstants.Timings.AdministrativelyDerivedFailureDetection,
			HighAvailabilityConstants.Timings.AdministrativelyDerivedFailureDetection,
			HighAvailabilityConstants.Timings.ServerInMaintenanceModeTurnaroundTime
		};

		private enum Timings
		{
			ReqdDataProtectionInfrastructureDetection = 120,
			ReplayRestartWindow = 300,
			ReqdDataProtectionInfrastructureBugcheck = 600,
			ReqdDataProtectionInfrastructureEscalate2 = 1200,
			NonHealthThreatingDataProtectionInfraDetection = 300,
			TransientNonHealthThreatingDataProtectionInfraDetection = 3600,
			NonHealthThreatingDataProtectionInfraRepair1 = 3600,
			NonHealthThreatingDataProtectionInfraRepair2 = 18000,
			ReqdDataProtectionInfrastructureEscalate1 = 600,
			ReqdDataProtectionInfrastructureRecoveryFailure = 600,
			TransientSuppressedLoadDetectionWindow = 7200,
			AdministrativelyDerivedFailureDetection = 28800,
			EstimatedReseedTime = 28800,
			ServerInMaintenanceModeTurnaroundTime = 259200,
			ProbeFailureDuration = 3600
		}
	}
}
