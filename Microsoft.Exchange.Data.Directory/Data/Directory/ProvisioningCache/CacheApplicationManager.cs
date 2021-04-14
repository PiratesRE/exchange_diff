using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal static class CacheApplicationManager
	{
		static CacheApplicationManager()
		{
			CacheApplicationManager.appDiagPipes.Add("Powershell", "Powershell-NamedPipe");
			CacheApplicationManager.appDiagPipes.Add("Powershell-LiveId", "Powershell-LiveId-NamedPipe");
			CacheApplicationManager.appDiagPipes.Add("Powershell-Proxy", "Powershell-Proxy-NamedPipe");
			CacheApplicationManager.appDiagPipes.Add("PowershellLiveId-Proxy", "PowershellLiveId-Proxy-NamedPipe");
			CacheApplicationManager.appDiagPipes.Add("Ecp", "Ecp-NamedPipe");
			CacheApplicationManager.appDiagPipes.Add("Psws", "Psws-NamedPipe");
			CacheApplicationManager.appDiagPipes.Add("Owa", "Owa-NamedPipe");
			CacheApplicationManager.appDiagPipes.Add("HxS", "HxS-NamedPipe");
		}

		public static string GetAppPipeName(string app)
		{
			if (!CacheApplicationManager.IsApplicationDefined(app))
			{
				throw new ArgumentException(string.Format("The application {0} is not defined in Exchange ProvisioningCache.", app.ToString()));
			}
			return CacheApplicationManager.appDiagPipes[app];
		}

		public static bool IsApplicationDefined(string app)
		{
			return CacheApplicationManager.appDiagPipes.ContainsKey(app);
		}

		internal const string RPSIdentification = "Powershell";

		internal const string RPSLiveIdIdentification = "Powershell-LiveId";

		internal const string RPSProxyIdentification = "Powershell-Proxy";

		internal const string RPSLiveIdProxyIdentification = "PowershellLiveId-Proxy";

		internal const string EcpIdentification = "Ecp";

		internal const string PswsIdentification = "Psws";

		internal const string OwaIdentification = "Owa";

		internal const string HxsIdentification = "HxS";

		private static readonly Dictionary<string, string> appDiagPipes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}
}
