using System;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery
{
	internal static class ConnectionTypeClassification
	{
		public static bool IsExchangeServer(ConnectionSettingsType connectionSettingsType)
		{
			return connectionSettingsType == ConnectionSettingsType.ExchangeActiveSync;
		}
	}
}
