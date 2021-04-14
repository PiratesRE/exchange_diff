using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class ThrottleHelper : IThrottleHelper
	{
		public string[] GetServersInGroup(string groupName)
		{
			return ExchangeThrottleSettings.ResolveKnownExchangeGroupToServers(groupName);
		}

		public int GetServerVersion(string serverName)
		{
			Server exchangeServerByName = DirectoryAccessor.Instance.GetExchangeServerByName(serverName);
			if (exchangeServerByName != null)
			{
				return exchangeServerByName.AdminDisplayVersion.ToInt();
			}
			throw new ADOperationException(Strings.ServerVersionNotFound(serverName));
		}

		public ThrottleSettingsBase Settings
		{
			get
			{
				return ExchangeThrottleSettings.Instance;
			}
		}

		public GlobalTunables Tunables
		{
			get
			{
				if (ThrottleHelper.globalTunables != null)
				{
					return ThrottleHelper.globalTunables;
				}
				GlobalTunables globalTunables = new GlobalTunables
				{
					LocalMachineName = Environment.MachineName,
					ThrottleGroupCacheRefreshFrequency = TimeSpan.FromMinutes(5.0),
					ThrottleGroupCacheRefreshStartDelay = TimeSpan.FromMinutes(1.0),
					ThrottlingV2SupportedServerVersion = new ServerVersion(15, 0, 785, 0).ToInt(),
					IsRunningMock = false
				};
				ThrottleHelper.globalTunables = globalTunables;
				return ThrottleHelper.globalTunables;
			}
		}

		private static GlobalTunables globalTunables;
	}
}
