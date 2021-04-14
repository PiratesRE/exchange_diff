using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ProcessInfoEventLogger
	{
		static ProcessInfoEventLogger()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				ProcessInfoEventLogger.ProcessName = currentProcess.ProcessName;
				ProcessInfoEventLogger.ProcessId = currentProcess.Id;
			}
		}

		public static void Log(ExEventLog.EventTuple tuple, string periodicKey, params object[] arguments)
		{
			object[] array = new object[arguments.Length + 3];
			array[0] = ProcessInfoEventLogger.ProcessName;
			array[1] = ProcessInfoEventLogger.ProcessId;
			array[2] = Environment.CurrentManagedThreadId;
			arguments.CopyTo(array, 3);
			StorageGlobals.EventLogger.LogEvent(tuple, periodicKey, array);
		}

		private static readonly string ProcessName;

		private static readonly int ProcessId;
	}
}
