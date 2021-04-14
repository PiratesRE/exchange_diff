using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ConnectionDroppedNotifier : PendingRequestNotifierBase
	{
		public ConnectionDroppedNotifier(IMailboxContext userContext) : base(userContext)
		{
			this.mailboxSessionDisplayName = userContext.ExchangePrincipal.MailboxInfo.DisplayName;
			this.payloadString = string.Empty;
		}

		internal override void PickupData()
		{
			if (!string.IsNullOrEmpty(this.payloadString))
			{
				base.FireDataAvailableEvent();
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "ConnectionDroppedPayload.PickupData. DataAvailable method called. Mailbox: {0}", this.mailboxSessionDisplayName);
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "ConnectionDroppedPayload.PickupData. No need to call DataAvailable method. Mailbox: {0}", this.mailboxSessionDisplayName);
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			return null;
		}

		private readonly string mailboxSessionDisplayName;

		private readonly string payloadString;
	}
}
