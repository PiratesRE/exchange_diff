using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class SubscriptionPayload : IPendingRequestNotifier
	{
		public SubscriptionPayload(UserContext userContext, MailboxSession mailboxSession, SubscriptionNotificationHandler notificationHandler)
		{
			this.payloadString = new StringBuilder(256);
			this.userContext = userContext;
			this.mailboxSessionDisplayName = string.Copy(mailboxSession.DisplayName);
			this.notificationHandler = notificationHandler;
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
			string result = string.Empty;
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionPayload.ReadDataAndResetState. Mailbox: {0}", this.mailboxSessionDisplayName);
			lock (this)
			{
				this.containsDataToPickup = false;
				if (0 < this.payloadString.Length)
				{
					result = this.payloadString.ToString();
					this.Clear();
				}
			}
			return result;
		}

		public void PickupData()
		{
			bool flag = false;
			lock (this)
			{
				flag = (!this.containsDataToPickup && 0 < this.payloadString.Length);
				if (flag)
				{
					this.containsDataToPickup = true;
				}
			}
			if (flag)
			{
				this.DataAvailable(this, new EventArgs());
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionPayload.PickupData. DataAvailable method called. Mailbox: {0}", this.mailboxSessionDisplayName);
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionPayload.PickupData. No need to call DataAvailable method. Mailbox: {0}", this.mailboxSessionDisplayName);
		}

		public void Clear()
		{
			lock (this)
			{
				this.payloadString.Remove(0, this.payloadString.Length);
			}
		}

		public void ConnectionAliveTimer()
		{
			this.notificationHandler.HandlePendingGetTimerCallback();
		}

		public void AddPayload(StringBuilder payloadBuilder)
		{
			lock (this)
			{
				this.payloadString.Append(payloadBuilder.ToString());
			}
		}

		internal void RegisterWithPendingRequestNotifier()
		{
			if (this.userContext != null && this.userContext.PendingRequestManager != null)
			{
				this.userContext.PendingRequestManager.AddPendingRequestNotifier(this);
			}
		}

		private const int DefaultPayloadStringSize = 256;

		private bool containsDataToPickup;

		private StringBuilder payloadString;

		private UserContext userContext;

		private SubscriptionNotificationHandler notificationHandler;

		private string mailboxSessionDisplayName;
	}
}
