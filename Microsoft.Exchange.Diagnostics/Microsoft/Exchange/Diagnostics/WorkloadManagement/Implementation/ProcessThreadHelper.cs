using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal static class ProcessThreadHelper
	{
		internal static ProcessThread Current
		{
			get
			{
				ProcessThread processThread = null;
				Dictionary<int, ProcessThread> refreshedMap = ProcessThreadHelper.threadsMap;
				int currentThreadId = DiagnosticsNativeMethods.GetCurrentThreadId();
				refreshedMap.TryGetValue(currentThreadId, out processThread);
				if (processThread == null || processThread.Id != currentThreadId)
				{
					refreshedMap = ProcessThreadHelper.GetRefreshedMap();
					processThread = refreshedMap[currentThreadId];
					ProcessThreadHelper.threadsMap = refreshedMap;
				}
				return processThread;
			}
		}

		private static Dictionary<int, ProcessThread> GetRefreshedMap()
		{
			Dictionary<int, ProcessThread> dictionary = new Dictionary<int, ProcessThread>();
			Process currentProcess = Process.GetCurrentProcess();
			foreach (object obj in currentProcess.Threads)
			{
				ProcessThread processThread = (ProcessThread)obj;
				dictionary.Add(processThread.Id, processThread);
			}
			return dictionary;
		}

		private static Dictionary<int, ProcessThread> threadsMap = new Dictionary<int, ProcessThread>();
	}
}
