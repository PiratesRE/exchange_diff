using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class HierarchyNotificationHandler : MapiNotificationHandlerBase
	{
		public HierarchyNotificationHandler(string subscriptionId, UserContext userContext, Guid mailboxGuid) : base(subscriptionId, userContext, false)
		{
			this.mailboxGuid = mailboxGuid;
			this.hierarchyNotifier = new HierarchyNotifier(subscriptionId, userContext);
			this.hierarchyNotifier.RegisterWithPendingRequestNotifier();
		}

		internal override void HandleNotificationInternal(Notification notif, MapiNotificationsLogEvent logEvent, object context)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "HierarchyNotificationHandler.HandleNotification Hierarchy notification received. User: {0}", base.UserContext.PrimarySmtpAddress);
			lock (base.SyncRoot)
			{
				QueryNotification queryNotification = notif as QueryNotification;
				if (queryNotification == null)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "HierarchyNotificationHandler.HandleNotification: Received a null Hierarchy Notification object for user {0}", base.UserContext.PrimarySmtpAddress);
					logEvent.NullNotification = true;
				}
				else if (queryNotification.Row.Length < this.querySubscriptionProperties.Length)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "HierarchyNotificationHandler.HandleNotification: Received an incomplete Hierarchy Notification object for user {0}", base.UserContext.PrimarySmtpAddress);
					logEvent.InvalidNotification = true;
				}
				else
				{
					switch (queryNotification.EventType)
					{
					case QueryNotificationType.QueryResultChanged:
					case QueryNotificationType.Reload:
						this.hierarchyNotifier.AddRefreshPayload(base.SubscriptionId);
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "HierarchyNotificationHandler.HandleNotification: Hierarchy notification refresh payload for user: {0}", base.UserContext.PrimarySmtpAddress);
						break;
					case QueryNotificationType.Error:
						this.hierarchyNotifier.AddRefreshPayload(base.SubscriptionId);
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "HierarchyNotificationHandler.HandleNotification: Error in Hierarchy Notification, Type is QueryNotificationType.Error, user: {0}", base.UserContext.PrimarySmtpAddress);
						break;
					case QueryNotificationType.RowAdded:
					case QueryNotificationType.RowDeleted:
					case QueryNotificationType.RowModified:
						this.ProcessHierarchyNotification(queryNotification);
						break;
					}
					this.hierarchyNotifier.PickupData();
				}
			}
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "HierarchyNotificationHandler.HandlePendingGetTimerCallback Start");
			lock (base.SyncRoot)
			{
				base.InitSubscription();
				if (base.MissedNotifications)
				{
					base.NeedRefreshPayload = true;
				}
				base.MissedNotifications = false;
			}
			if (base.NeedRefreshPayload)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "HierarchyNotificationHandler.HandlePendingGetTimerCallback NeedRefreshPayload");
				this.hierarchyNotifier.AddRefreshPayload(base.SubscriptionId);
				base.NeedRefreshPayload = false;
			}
			this.hierarchyNotifier.PickupData();
		}

		protected override void InitSubscriptionInternal()
		{
			if (!base.UserContext.MailboxSessionLockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method HierarchyNotificationHandler.InitSubscriptionInternal");
			}
			this.favoriteSendersFolderId = base.UserContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.FromFavoriteSenders);
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "HierarchyNotificationHandler.InitSubscriptionInternal Start");
			using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, DefaultFolderType.Configuration))
			{
				base.QueryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, this.querySubscriptionProperties);
				base.QueryResult.GetRows(base.QueryResult.EstimatedRowCount);
				base.Subscription = Subscription.Create(base.QueryResult, new NotificationHandler(base.HandleNotification));
			}
		}

		protected virtual bool IsAllowedSearchFolder(StoreObjectId folderId)
		{
			return this.favoriteSendersFolderId != null && this.favoriteSendersFolderId.Equals(folderId);
		}

		protected virtual HierarchyNotificationPayload GetPayloadInstance(StoreObjectId folderId)
		{
			return new HierarchyNotificationPayload();
		}

		private void ProcessHierarchyNotification(QueryNotification notification)
		{
			VersionedId versionedId = notification.Row[0] as VersionedId;
			StoreObjectId storeObjectId = null;
			if (versionedId != null)
			{
				storeObjectId = versionedId.ObjectId;
			}
			if (storeObjectId == null || notification.Row[2] == null || notification.Row[3] == null || notification.Row[4] == null || notification.Row[6] == null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "Hierarchy notification has a null folder id or is malformed. User: {0}", base.UserContext.PrimarySmtpAddress);
				return;
			}
			int num = (int)notification.Row[2];
			int num2 = (int)notification.Row[3];
			if (notification.Row[6] is bool && !(bool)notification.Row[6] && !this.IsAllowedSearchFolder(storeObjectId))
			{
				return;
			}
			HierarchyNotificationPayload payloadInstance = this.GetPayloadInstance(storeObjectId);
			payloadInstance.FolderId = StoreId.StoreIdToEwsId(this.mailboxGuid, storeObjectId);
			payloadInstance.DisplayName = (string)notification.Row[1];
			payloadInstance.ParentFolderId = ((notification.Row[5] != null) ? StoreId.StoreIdToEwsId(this.mailboxGuid, notification.Row[5] as StoreId) : null);
			payloadInstance.ItemCount = (long)num;
			payloadInstance.UnreadCount = (long)num2;
			payloadInstance.SubscriptionId = base.SubscriptionId;
			payloadInstance.EventType = notification.EventType;
			if (!(notification.Row[4] is PropertyError))
			{
				string itemClass = (string)notification.Row[4];
				StoreObjectType objectType = ObjectClass.GetObjectType(itemClass);
				payloadInstance.FolderType = objectType;
			}
			else
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "Hierarchy notification received PropertyError for Item Class. User: {0}", base.UserContext.PrimarySmtpAddress);
			}
			this.hierarchyNotifier.AddFolderCountPayload(storeObjectId, payloadInstance);
		}

		private readonly Guid mailboxGuid;

		private StoreObjectId favoriteSendersFolderId;

		private PropertyDefinition[] querySubscriptionProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.DisplayName,
			FolderSchema.ItemCount,
			FolderSchema.UnreadCount,
			StoreObjectSchema.ContainerClass,
			StoreObjectSchema.ParentItemId,
			FolderSchema.IPMFolder
		};

		private HierarchyNotifier hierarchyNotifier;
	}
}
