using System;
using System.Web;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class RPSPerfCounterHelper
	{
		public static RemotePowershellPerformanceCountersInstance CurrentPerfCounter
		{
			get
			{
				if (RPSPerfCounterHelper.current == null)
				{
					string text = null;
					try
					{
						text = HttpRuntime.AppDomainAppPath;
					}
					catch (ArgumentNullException)
					{
					}
					string instanceName;
					if (text != null && text.IndexOf("powershell-liveid-proxy", StringComparison.OrdinalIgnoreCase) != -1)
					{
						instanceName = "RemotePS-LiveID";
					}
					else if (text != null && text.IndexOf("powershell-proxy", StringComparison.OrdinalIgnoreCase) != -1)
					{
						instanceName = "RemotePS";
					}
					else
					{
						instanceName = "RemotePS-NoPro";
					}
					RPSPerfCounterHelper.current = RemotePowershellPerformanceCounters.GetInstance(instanceName);
				}
				return RPSPerfCounterHelper.current;
			}
		}

		public static RemotePowershellPerformanceCountersInstance GetPerfCounterForAuthZ(string vdirName, int port)
		{
			string instanceName = "RemotePS-NoPro";
			if (vdirName != null && vdirName.Equals("/powershell", StringComparison.InvariantCultureIgnoreCase) && port == 444)
			{
				instanceName = "RemotePS";
			}
			else if (vdirName != null && vdirName.Equals("/powershell-liveid", StringComparison.InvariantCultureIgnoreCase))
			{
				instanceName = "RemotePS-LiveID";
			}
			return RemotePowershellPerformanceCounters.GetInstance(instanceName);
		}

		private const string PSPerfCounterInstanceName = "RemotePS";

		private const string PSLiveIdPerfCounterInstanceName = "RemotePS-LiveID";

		private const string PSNoProxyPerfCounterInstanceName = "RemotePS-NoPro";

		private static RemotePowershellPerformanceCountersInstance current;
	}
}
