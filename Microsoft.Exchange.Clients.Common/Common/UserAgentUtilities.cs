using System;

namespace Microsoft.Exchange.Clients.Common
{
	public static class UserAgentUtilities
	{
		public static bool IsMonitoringRequest(string userAgent)
		{
			return !string.IsNullOrEmpty(userAgent) && userAgent.IndexOf("MSEXCHMON", StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}
