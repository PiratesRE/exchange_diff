using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.GroupMailbox;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UnseenItemNotifier : PendingRequestNotifierBase
	{
		internal UnseenItemNotifier(string subscriptionId, IMailboxContext userContext, string storeSubscriptionId, UserMailboxLocator userMailboxLocator) : base(userContext)
		{
			this.userMailboxLocator = userMailboxLocator;
			this.payloadSubscriptionId = subscriptionId;
			base.SubscriptionId = storeSubscriptionId;
		}

		internal UserMailboxLocator UserMailboxLocator
		{
			get
			{
				return this.userMailboxLocator;
			}
		}

		internal string PayloadSubscriptionId
		{
			get
			{
				return this.payloadSubscriptionId;
			}
		}

		internal void AddGroupNotificationPayload(UnseenItemNotificationPayload payload)
		{
			lock (this)
			{
				if (this.unseenItemPayload == null)
				{
					NotificationStatisticsManager.Instance.NotificationCreated(payload);
				}
				else if (payload != null)
				{
					payload.CreatedTime = this.unseenItemPayload.CreatedTime;
				}
				this.unseenItemPayload = payload;
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			if (this.unseenItemPayload != null)
			{
				list.Add(this.unseenItemPayload);
				this.unseenItemPayload = null;
			}
			return list;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.unseenItemPayload != null;
		}

		private readonly UserMailboxLocator userMailboxLocator;

		private UnseenItemNotificationPayload unseenItemPayload;

		private readonly string payloadSubscriptionId;
	}
}
