using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class ConnectedAccountsNotification : ServiceCommand<bool>
	{
		public ConnectedAccountsNotification(CallContext callContext, bool isOWALogon) : base(callContext)
		{
			this.isOWALogon = isOWALogon;
		}

		protected override bool InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			List<IConnectedAccountsNotificationManager> connectedAccountNotificationManagers = userContext.GetConnectedAccountNotificationManagers(base.MailboxIdentityMailboxSession);
			foreach (IConnectedAccountsNotificationManager notificationManager in connectedAccountNotificationManagers)
			{
				this.SendSyncNowNotification(notificationManager);
			}
			return true;
		}

		private void SendSyncNowNotification(IConnectedAccountsNotificationManager notificationManager)
		{
			if (this.isOWALogon)
			{
				notificationManager.SendLogonTriggeredSyncNowRequest();
				ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "ConnectedAccountsNotification.SendSyncNowNotification - ConnectedAccountsNotificationManager was setup and SendLogonTriggeredSyncNowRequest invoked.");
				return;
			}
			notificationManager.SendRefreshButtonTriggeredSyncNowRequest();
			ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "ConnectedAccountsNotification.SendSyncNowNotification - ConnectedAccountsNotificationManager is setup and SendRefreshButtonTriggeredSyncNowRequest invoked.");
		}

		private readonly bool isOWALogon;
	}
}
