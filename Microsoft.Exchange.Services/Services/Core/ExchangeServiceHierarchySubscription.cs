using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExchangeServiceHierarchySubscription : ExchangeServiceSubscription
	{
		internal ExchangeServiceHierarchySubscription(string subscriptionId) : base(subscriptionId)
		{
		}

		internal Guid MailboxGuid { get; set; }

		internal Subscription Subscription { get; set; }

		internal Action<HierarchyNotification> Callback { get; set; }

		internal QueryResult QueryResult { get; set; }

		internal override void HandleNotification(Notification notification)
		{
			HierarchyNotification hierarchyNotification = null;
			if (notification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceHierarchySubscription.HandleNotification: Received a null notification for subscriptionId: {0}", base.SubscriptionId);
				return;
			}
			QueryNotification queryNotification = notification as QueryNotification;
			if (queryNotification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceHierarchySubscription.HandleNotification: Received a notification of an unknown type for subscriptionId: {0}", base.SubscriptionId);
				return;
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ExchangeServiceHierarchySubscription.HandleNotification: Received a {0} notification for subscriptionId: {1}", queryNotification.EventType.ToString(), base.SubscriptionId);
			switch (queryNotification.EventType)
			{
			case QueryNotificationType.RowAdded:
			case QueryNotificationType.RowModified:
				hierarchyNotification = this.ProcessHierarchyNotification(queryNotification, (queryNotification.EventType == QueryNotificationType.RowAdded) ? NotificationTypeType.Create : NotificationTypeType.Update);
				goto IL_10B;
			case QueryNotificationType.RowDeleted:
				hierarchyNotification = new HierarchyNotification
				{
					InstanceKey = queryNotification.Index,
					NotificationType = NotificationTypeType.Delete
				};
				goto IL_10B;
			case QueryNotificationType.Reload:
				hierarchyNotification = new HierarchyNotification
				{
					InstanceKey = queryNotification.Index,
					NotificationType = NotificationTypeType.Reload
				};
				goto IL_10B;
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceHierarchySubscription.HandleNotification: Unknown notification event type");
			IL_10B:
			if (hierarchyNotification != null)
			{
				this.Callback(hierarchyNotification);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceHierarchySubscription.HandleNotification: Returned from callback");
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (this.Subscription != null)
			{
				this.Subscription.Dispose();
				this.Subscription = null;
			}
			if (this.QueryResult != null)
			{
				this.QueryResult.Dispose();
				this.QueryResult = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeServiceHierarchySubscription>(this);
		}

		private HierarchyNotification ProcessHierarchyNotification(QueryNotification notification, NotificationTypeType notificationType)
		{
			HierarchyNotification hierarchyNotification = new HierarchyNotification();
			hierarchyNotification.InstanceKey = notification.Index;
			VersionedId versionedId = notification.Row[0] as VersionedId;
			StoreObjectId storeObjectId = null;
			if (versionedId != null)
			{
				storeObjectId = versionedId.ObjectId;
			}
			if (storeObjectId == null || notification.Row[2] == null || notification.Row[3] == null || notification.Row[4] == null || notification.Row[6] == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Hierarchy notification has a null folder id or is malformed.");
				return null;
			}
			int num = (int)notification.Row[2];
			int num2 = (int)notification.Row[3];
			if (!(notification.Row[6] is bool) || !(bool)notification.Row[6])
			{
				return null;
			}
			hierarchyNotification.NotificationType = notificationType;
			hierarchyNotification.FolderId = new FolderId(StoreId.StoreIdToEwsId(this.MailboxGuid, storeObjectId), null);
			hierarchyNotification.DisplayName = (string)notification.Row[1];
			hierarchyNotification.ParentFolderId = ((notification.Row[5] != null) ? new FolderId(StoreId.StoreIdToEwsId(this.MailboxGuid, notification.Row[5] as StoreId), null) : null);
			hierarchyNotification.ItemCount = (long)num;
			hierarchyNotification.UnreadCount = (long)num2;
			hierarchyNotification.IsHidden = (notification.Row[7] is bool && (bool)notification.Row[7]);
			if (!(notification.Row[4] is PropertyError))
			{
				hierarchyNotification.FolderClass = (string)notification.Row[4];
				hierarchyNotification.FolderType = ObjectClass.GetObjectType(hierarchyNotification.FolderClass);
			}
			else
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Hierarchy notification received PropertyError for Item Class.");
			}
			return hierarchyNotification;
		}
	}
}
