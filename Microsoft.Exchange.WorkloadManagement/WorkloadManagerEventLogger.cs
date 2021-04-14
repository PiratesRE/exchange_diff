using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal static class WorkloadManagerEventLogger
	{
		public static void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			if (string.IsNullOrEmpty(WorkloadManagerEventLogger.processName))
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					WorkloadManagerEventLogger.processName = currentProcess.ProcessName;
					WorkloadManagerEventLogger.processId = currentProcess.Id;
				}
			}
			object[] array = new object[messageArgs.Length + 2];
			array[0] = WorkloadManagerEventLogger.processName;
			array[1] = WorkloadManagerEventLogger.processId;
			Array.Copy(messageArgs, 0, array, 2, messageArgs.Length);
			WorkloadManagerEventLogger.logger.LogEvent(tuple, periodicKey, array);
		}

		private static readonly ExEventLog logger = new ExEventLog(ExTraceGlobals.CommonTracer.Category, "MSExchange WorkloadManagement");

		private static string processName = string.Empty;

		private static int processId;
	}
}
