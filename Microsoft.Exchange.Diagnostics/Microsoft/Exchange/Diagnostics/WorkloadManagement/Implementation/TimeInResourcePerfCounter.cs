using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal static class TimeInResourcePerfCounter
	{
		static TimeInResourcePerfCounter()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				string processName = currentProcess.ProcessName;
				string cachedAppName = ActivityCoverageReport.CachedAppName;
				string instanceName = string.Format("{0}-{1}-{2}", "AD", processName, cachedAppName);
				TimeInResourcePerfCounter.adInstance = MSExchangeActivityContext.GetInstance(instanceName);
				string instanceName2 = string.Format("{0}-{1}-{2}", DisplayNameAttribute.GetEnumName(ActivityOperationType.MailboxCall), processName, cachedAppName);
				TimeInResourcePerfCounter.mailboxInstance = MSExchangeActivityContext.GetInstance(instanceName2);
				string instanceName3 = string.Format("{0}-{1}-{2}", DisplayNameAttribute.GetEnumName(ActivityOperationType.ExRpcAdmin), processName, cachedAppName);
				TimeInResourcePerfCounter.exRpcAdminInstance = MSExchangeActivityContext.GetInstance(instanceName3);
			}
		}

		internal static void AddOperation(ActivityOperationType activityOperationType, float value)
		{
			MSExchangeActivityContextInstance instance = TimeInResourcePerfCounter.GetInstance(activityOperationType);
			if (instance != null)
			{
				instance.TimeInResourcePerSecond.IncrementBy((long)value);
			}
		}

		private static MSExchangeActivityContextInstance GetInstance(ActivityOperationType operation)
		{
			MSExchangeActivityContextInstance result = null;
			switch (operation)
			{
			case ActivityOperationType.ADRead:
			case ActivityOperationType.ADSearch:
			case ActivityOperationType.ADWrite:
				result = TimeInResourcePerfCounter.adInstance;
				break;
			case ActivityOperationType.MailboxCall:
				result = TimeInResourcePerfCounter.mailboxInstance;
				break;
			case ActivityOperationType.ExRpcAdmin:
				result = TimeInResourcePerfCounter.exRpcAdminInstance;
				break;
			}
			return result;
		}

		private const string InstanceFormat = "{0}-{1}-{2}";

		private const string AdOpName = "AD";

		private static readonly MSExchangeActivityContextInstance adInstance;

		private static readonly MSExchangeActivityContextInstance mailboxInstance;

		private static readonly MSExchangeActivityContextInstance exRpcAdminInstance;
	}
}
