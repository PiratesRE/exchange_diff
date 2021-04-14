using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class UserWorkloadManagerHandler : ExchangeDiagnosableWrapper<UserWorkloadManagerResult>
	{
		private UserWorkloadManagerHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "UserWorkloadManager";
			}
		}

		protected override string UsageText
		{
			get
			{
				return "This diagnostics handler returns detailed information of the inner workings of the WLM. Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: For a “no cache dump” invocation:\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component UserWorkloadManager\r\n                        \r\n                        Example 2:For a “cache dump” invocation:\r\n                        Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component UserWorkloadManager -Argument dumpcache";
			}
		}

		public static UserWorkloadManagerHandler GetInstance()
		{
			if (UserWorkloadManagerHandler.instance == null)
			{
				lock (UserWorkloadManagerHandler.lockObject)
				{
					if (UserWorkloadManagerHandler.instance == null)
					{
						UserWorkloadManagerHandler.instance = new UserWorkloadManagerHandler();
					}
				}
			}
			return UserWorkloadManagerHandler.instance;
		}

		internal override UserWorkloadManagerResult GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			bool dumpCaches = !string.IsNullOrEmpty(argument.Argument) && argument.Argument.ToLower() == "dumpcache";
			return UserWorkloadManager.Singleton.GetDiagnosticSnapshot(dumpCaches);
		}

		private const string DumpCacheArgument = "dumpcache";

		private static UserWorkloadManagerHandler instance;

		private static object lockObject = new object();
	}
}
