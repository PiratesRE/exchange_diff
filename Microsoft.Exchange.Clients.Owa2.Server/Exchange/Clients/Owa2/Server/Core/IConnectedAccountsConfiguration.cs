using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface IConnectedAccountsConfiguration
	{
		bool NotificationsEnabled { get; }

		bool LogonTriggeredSyncNowEnabled { get; }

		bool RefreshButtonTriggeredSyncNowEnabled { get; }

		TimeSpan RefreshButtonTriggeredSyncNowSuppressThreshold { get; }

		bool PeriodicSyncNowEnabled { get; }

		TimeSpan PeriodicSyncNowInterval { get; }
	}
}
