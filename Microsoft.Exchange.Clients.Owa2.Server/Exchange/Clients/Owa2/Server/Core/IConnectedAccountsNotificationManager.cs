using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface IConnectedAccountsNotificationManager : IDisposable
	{
		void SendLogonTriggeredSyncNowRequest();

		void SendRefreshButtonTriggeredSyncNowRequest();
	}
}
