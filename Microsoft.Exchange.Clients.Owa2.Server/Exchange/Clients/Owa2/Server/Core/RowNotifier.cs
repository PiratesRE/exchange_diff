using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RowNotifier : PendingRequestNotifierBase
	{
		public RowNotifier(string subscriptionId, IMailboxContext context, Guid mailboxGuid) : base(subscriptionId, context)
		{
			this.queue = new Queue<NotificationPayloadBase>();
			this.mailboxGuid = mailboxGuid;
		}

		protected Queue<NotificationPayloadBase> Queue
		{
			get
			{
				return this.queue;
			}
		}

		internal void AddFolderRefreshPayload(StoreObjectId folderId, string subscriptionId)
		{
			RowNotificationPayload rowNotificationPayload = new RowNotificationPayload();
			rowNotificationPayload.FolderId = StoreId.StoreIdToEwsId(this.mailboxGuid, folderId);
			rowNotificationPayload.EventType = QueryNotificationType.Reload;
			rowNotificationPayload.SubscriptionId = subscriptionId;
			rowNotificationPayload.Source = new MailboxLocation(this.mailboxGuid);
			NotificationStatisticsManager.Instance.NotificationDropped(this.queue, NotificationState.CreatedOrReceived);
			this.ClearRowNotificationPayload();
			this.queue.Enqueue(rowNotificationPayload);
			NotificationStatisticsManager.Instance.NotificationCreated(rowNotificationPayload);
		}

		internal void AddQueryResultChangedPayload(StoreObjectId folderId, string subscriptionId)
		{
			RowNotificationPayload rowNotificationPayload = new RowNotificationPayload();
			rowNotificationPayload.FolderId = StoreId.StoreIdToEwsId(this.mailboxGuid, folderId);
			rowNotificationPayload.EventType = QueryNotificationType.QueryResultChanged;
			rowNotificationPayload.SubscriptionId = subscriptionId;
			rowNotificationPayload.Source = new MailboxLocation(this.mailboxGuid);
			this.queue.Enqueue(rowNotificationPayload);
			NotificationStatisticsManager.Instance.NotificationCreated(rowNotificationPayload);
		}

		internal void AddFolderContentChangePayload(StoreObjectId folderId, NotificationPayloadBase payload)
		{
			lock (this)
			{
				if (this.queue != null && this.queue.Count >= 40)
				{
					NotificationStatisticsManager.Instance.NotificationCreated(payload);
					NotificationStatisticsManager.Instance.NotificationDropped(payload, NotificationState.CreatedOrReceived);
					this.AddFolderRefreshPayload(folderId, payload.SubscriptionId);
				}
				else
				{
					this.queue.Enqueue(payload);
					NotificationStatisticsManager.Instance.NotificationCreated(payload);
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			if (this.queue.Count > 0)
			{
				foreach (NotificationPayloadBase item in this.queue)
				{
					list.Add(item);
				}
				this.queue.Clear();
			}
			return list;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.AreThereNotifications();
		}

		private bool AreThereNotifications()
		{
			return this.queue.Count > 0;
		}

		private void ClearRowNotificationPayload()
		{
			lock (this)
			{
				if (this.queue != null)
				{
					this.queue.Clear();
				}
			}
		}

		protected const int MaxFolderContentNotificationQueueSize = 40;

		private readonly Guid mailboxGuid;

		private Queue<NotificationPayloadBase> queue;
	}
}
