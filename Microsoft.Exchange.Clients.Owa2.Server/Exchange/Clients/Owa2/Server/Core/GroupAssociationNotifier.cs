using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GroupAssociationNotifier : PendingRequestNotifierBase
	{
		internal GroupAssociationNotifier(string subscriptionId, IMailboxContext userContext) : base(subscriptionId, userContext)
		{
			this.refreshAll = false;
			this.payloadQueue = new Queue<NotificationPayloadBase>();
		}

		internal void AddGroupAssociationNotificationPayload(NotificationPayloadBase payload)
		{
			lock (this)
			{
				if (this.refreshAll)
				{
					NotificationStatisticsManager.Instance.NotificationCreated(payload);
					NotificationStatisticsManager.Instance.NotificationDropped(payload, NotificationState.CreatedOrReceived);
				}
				else
				{
					this.payloadQueue.Enqueue(payload);
					NotificationStatisticsManager.Instance.NotificationCreated(payload);
				}
			}
		}

		internal void AddRefreshPayload()
		{
			lock (this)
			{
				if (!this.refreshAll)
				{
					NotificationStatisticsManager.Instance.NotificationDropped(this.payloadQueue, NotificationState.CreatedOrReceived);
					this.payloadQueue.Clear();
					this.refreshAll = true;
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> result;
			if (this.refreshAll)
			{
				GroupAssociationNotificationPayload groupAssociationNotificationPayload = new GroupAssociationNotificationPayload();
				groupAssociationNotificationPayload.EventType = QueryNotificationType.Reload;
				groupAssociationNotificationPayload.Source = MailboxLocation.FromMailboxContext(base.UserContext);
				this.refreshAll = false;
				result = new List<NotificationPayloadBase>
				{
					groupAssociationNotificationPayload
				};
				NotificationStatisticsManager.Instance.NotificationCreated(groupAssociationNotificationPayload);
			}
			else
			{
				result = new List<NotificationPayloadBase>(this.payloadQueue);
				this.payloadQueue.Clear();
			}
			return result;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.payloadQueue.Count > 0 || this.refreshAll;
		}

		private bool refreshAll;

		private readonly Queue<NotificationPayloadBase> payloadQueue;
	}
}
