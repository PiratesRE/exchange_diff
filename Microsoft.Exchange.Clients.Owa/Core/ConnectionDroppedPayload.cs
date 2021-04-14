using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ConnectionDroppedPayload : IPendingRequestNotifier
	{
		public ConnectionDroppedPayload(UserContext userContext, MailboxSession mailboxSession, ConnectionDroppedNotificationHandler notificationHandler)
		{
			this.userContext = userContext;
			this.mailboxSessionDisplayName = string.Copy(mailboxSession.DisplayName);
			this.notificationHandler = notificationHandler;
			this.payloadString = string.Empty;
		}

		public event DataAvailableEventHandler DataAvailable;

		public bool ShouldThrottle
		{
			get
			{
				return true;
			}
		}

		public string ReadDataAndResetState()
		{
			return string.Empty;
		}

		public void PickupData()
		{
			if (!string.IsNullOrEmpty(this.payloadString))
			{
				this.DataAvailable(this, new EventArgs());
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "ConnectionDroppedPayload.PickupData. DataAvailable method called. Mailbox: {0}", this.mailboxSessionDisplayName);
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "ConnectionDroppedPayload.PickupData. No need to call DataAvailable method. Mailbox: {0}", this.mailboxSessionDisplayName);
		}

		public void ConnectionAliveTimer()
		{
			this.notificationHandler.HandlePendingGetTimerCallback();
		}

		internal void RegisterWithPendingRequestNotifier()
		{
			if (this.userContext != null && this.userContext.PendingRequestManager != null)
			{
				this.userContext.PendingRequestManager.AddPendingRequestNotifier(this);
			}
		}

		private UserContext userContext;

		private ConnectionDroppedNotificationHandler notificationHandler;

		private string mailboxSessionDisplayName;

		private string payloadString;
	}
}
