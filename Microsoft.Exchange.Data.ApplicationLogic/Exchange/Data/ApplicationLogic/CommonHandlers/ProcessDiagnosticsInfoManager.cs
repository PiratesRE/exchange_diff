using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.CommonHandlers
{
	public sealed class ProcessDiagnosticsInfoManager : ExchangeDiagnosableWrapper<ProcessInfo>
	{
		protected override string UsageText
		{
			get
			{
				return "This is a generic handler to retrieve information about current process. A sample usage is as shown below:";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Get-ExchangeDiagnosticsInfo -Process <ProcessName> -Component ProcessInfo";
			}
		}

		public static ProcessDiagnosticsInfoManager GetInstance()
		{
			if (ProcessDiagnosticsInfoManager.instance == null)
			{
				lock (ProcessDiagnosticsInfoManager.lockObject)
				{
					if (ProcessDiagnosticsInfoManager.instance == null)
					{
						ProcessDiagnosticsInfoManager.instance = new ProcessDiagnosticsInfoManager();
					}
				}
			}
			return ProcessDiagnosticsInfoManager.instance;
		}

		private ProcessDiagnosticsInfoManager()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "ProcessInfo";
			}
		}

		internal override ProcessInfo GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			ProcessInfo result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = new ProcessInfo
				{
					ServerName = currentProcess.MachineName,
					ProcessID = currentProcess.Id,
					ThreadCount = currentProcess.Threads.Count,
					MemorySize = currentProcess.VirtualMemorySize64,
					ProcessUpTime = currentProcess.TotalProcessorTime.TotalHours,
					Version = "15.00.1497.015"
				};
			}
			return result;
		}

		private static ProcessDiagnosticsInfoManager instance;

		private static object lockObject = new object();
	}
}
