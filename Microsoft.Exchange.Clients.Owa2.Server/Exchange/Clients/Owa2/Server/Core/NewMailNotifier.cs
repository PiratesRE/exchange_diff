using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class NewMailNotifier : PendingRequestNotifierBase
	{
		internal NewMailNotifier(string subscriptionId, IMailboxContext userContext) : base(subscriptionId, userContext)
		{
		}

		internal NewMailNotificationPayload Payload
		{
			get
			{
				NewMailNotificationPayload result;
				lock (this.lockObject)
				{
					result = this.payload;
				}
				return result;
			}
			set
			{
				lock (this.lockObject)
				{
					this.payload = value;
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			lock (this.lockObject)
			{
				if (this.Payload != null)
				{
					list.Add(this.Payload);
					this.Payload = null;
				}
			}
			return list;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.Payload != null;
		}

		private NewMailNotificationPayload payload;

		private object lockObject = new object();
	}
}
