using System;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Mapi.Unmanaged;
using Microsoft.Win32;

namespace Microsoft.Exchange.Provisioning
{
	public static class ProvisioningPerformanceHelper
	{
		internal static LatencyDetectionContext StartLatencyDetection(Task task)
		{
			if (ProvisioningPerformanceHelper.enabled == 0 || task.CurrentTaskContext.InvocationInfo == null)
			{
				return null;
			}
			if (string.Compare(task.CurrentTaskContext.InvocationInfo.CommandName, "New-Mailbox", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(task.CurrentTaskContext.InvocationInfo.CommandName, "New-SyncMailbox", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(task.CurrentTaskContext.InvocationInfo.CommandName, "New-GroupMailbox", StringComparison.OrdinalIgnoreCase) != 0)
			{
				return null;
			}
			IPerformanceDataProvider[] providers = new IPerformanceDataProvider[]
			{
				PerformanceContext.Current,
				RpcDataProvider.Instance,
				TaskPerformanceData.InternalValidate,
				TaskPerformanceData.InternalStateReset,
				TaskPerformanceData.WindowsLiveIdProvisioningHandlerForNew,
				TaskPerformanceData.MailboxProvisioningHandler,
				TaskPerformanceData.AdminLogProvisioningHandler,
				TaskPerformanceData.OtherProvisioningHandlers,
				TaskPerformanceData.InternalProcessRecord,
				TaskPerformanceData.SaveInitial,
				TaskPerformanceData.ReadUpdated,
				TaskPerformanceData.SaveResult,
				TaskPerformanceData.ReadResult,
				TaskPerformanceData.WriteResult
			};
			return ProvisioningPerformanceHelper.latencyDetectionContextFactory.CreateContext(ProvisioningPerformanceHelper.applicationVersion, task.CurrentTaskContext.InvocationInfo.CommandName, providers);
		}

		internal static TaskPerformanceData[] StopLatencyDetection(LatencyDetectionContext latencyDetectionContext)
		{
			if (latencyDetectionContext == null)
			{
				return null;
			}
			TaskPerformanceData[] result = latencyDetectionContext.StopAndFinalizeCollection();
			if (latencyDetectionContext.Elapsed.TotalSeconds > (double)ProvisioningPerformanceHelper.threshold)
			{
				object[] array = new object[3];
				string text = latencyDetectionContext.ToString("s");
				if (text.Length > 32766)
				{
					text = text.Substring(0, 32766);
				}
				array[0] = ProvisioningPerformanceHelper.threshold;
				array[1] = text;
				array[2] = Environment.CurrentManagedThreadId;
				TaskLogger.LogEvent("All", ProvisioningPerformanceHelper.endTuple, array);
			}
			return result;
		}

		private static int GetConfigurationValue(string registryValueName, int defaultValue)
		{
			int result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ProvisioningCmdletOptics"))
			{
				if (registryKey != null)
				{
					result = (int)registryKey.GetValue(registryValueName, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public const string ThresholdValueName = "Threshold";

		public const string EnabledValueName = "Enabled";

		public const int DefaultThreshold = 20;

		public const string SettingsOverrideRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ProvisioningCmdletOptics";

		private static ExEventLog.EventTuple endTuple = TaskEventLogConstants.Tuple_ExecuteTaskScriptLatency;

		private static readonly string applicationVersion = typeof(ProvisioningPerformanceHelper).GetApplicationVersion();

		private static int threshold = ProvisioningPerformanceHelper.GetConfigurationValue("Threshold", 20);

		private static int enabled = ProvisioningPerformanceHelper.GetConfigurationValue("Enabled", 1);

		private static LatencyDetectionContextFactory latencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory("Provisioning Cmdlet Latency");
	}
}
