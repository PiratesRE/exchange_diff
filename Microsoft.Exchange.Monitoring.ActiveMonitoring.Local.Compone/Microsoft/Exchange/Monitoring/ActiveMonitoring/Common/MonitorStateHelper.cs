using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class MonitorStateHelper
	{
		internal static string PrepareRepairingSubkeyPath(string monitorName, string targetResource)
		{
			string arg = Utilities.NormalizeStringToValidFileOrRegistryKeyName(monitorName);
			string result;
			if (string.IsNullOrEmpty(targetResource))
			{
				result = string.Format("MonitorDefinition\\{0}", arg);
			}
			else
			{
				string arg2 = Utilities.NormalizeStringToValidFileOrRegistryKeyName(targetResource);
				result = string.Format("MonitorDefinition\\{0}\\{1}", arg, arg2);
			}
			return result;
		}

		internal static void SetMonitorRepairingTargetFailureTime(string monitorName, string targetResource, DateTime targettedFailureTime)
		{
			string propertValue = targettedFailureTime.ToString("o");
			string subkeyName = MonitorStateHelper.PrepareRepairingSubkeyPath(monitorName, targetResource);
			RegistryHelper.SetProperty<string>("_RepairTargetFailureTime", propertValue, subkeyName, null, true);
		}

		internal static void DeleteMonitorRepairingState(string monitorName, string targetResource)
		{
			string subkeyName = MonitorStateHelper.PrepareRepairingSubkeyPath(monitorName, targetResource);
			RegistryHelper.DeleteProperty("_RepairTargetFailureTime", subkeyName, null, true);
		}

		internal static DateTime GetMonitorRepairingTargetFailureTime(string monitorName, string targetResource)
		{
			DateTime result = DateTime.MinValue;
			string subkeyName = MonitorStateHelper.PrepareRepairingSubkeyPath(monitorName, targetResource);
			string property = RegistryHelper.GetProperty<string>("_RepairTargetFailureTime", null, subkeyName, null, false);
			if (!string.IsNullOrEmpty(property))
			{
				result = DateTime.Parse(property);
			}
			return result;
		}

		internal static bool IsMonitorRepairing(string monitorName, string targetResource, DateTime? firstAlertObservedTime)
		{
			bool result = false;
			if (firstAlertObservedTime != null)
			{
				DateTime monitorRepairingTargetFailureTime = MonitorStateHelper.GetMonitorRepairingTargetFailureTime(monitorName, targetResource);
				if (monitorRepairingTargetFailureTime != DateTime.MinValue && monitorRepairingTargetFailureTime == firstAlertObservedTime.Value)
				{
					result = true;
				}
			}
			return result;
		}

		internal static void SetMonitorIsRepairingFlag(string monitorName, string targetResource, bool isRepairing)
		{
			if (!isRepairing)
			{
				MonitorStateHelper.DeleteMonitorRepairingState(monitorName, targetResource);
				return;
			}
			RpcGetMonitorHealthStatus.RpcMonitorHealthEntry rpcMonitorHealthEntry = MonitorResultCacheManager.Instance.FindMonitorHealthEntry(monitorName, targetResource);
			if (rpcMonitorHealthEntry == null)
			{
				throw new RepairingIsNotSetSinceMonitorEntryIsNotFoundException(monitorName, targetResource);
			}
			MonitorAlertState monitorAlertState = MonitorAlertState.Unknown;
			if (Enum.TryParse<MonitorAlertState>(rpcMonitorHealthEntry.AlertValue, out monitorAlertState) && (monitorAlertState == MonitorAlertState.Degraded || monitorAlertState == MonitorAlertState.Unhealthy || monitorAlertState == MonitorAlertState.Repairing))
			{
				MonitorStateHelper.SetMonitorRepairingTargetFailureTime(monitorName, targetResource, rpcMonitorHealthEntry.FirstAlertObservedTime);
				return;
			}
			throw new RepairingIsNotApplicableForCurrentMonitorStateException(monitorName, targetResource, monitorAlertState.ToString());
		}

		public const string MonitorRepairingTargetFailureTime = "_RepairTargetFailureTime";
	}
}
