using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GroupAssociationNotificationHandler : MapiNotificationHandlerBase
	{
		internal GroupAssociationNotificationHandler(string subscriptionId, IMailboxContext userContext, IRecipientSession adSession) : base(subscriptionId, userContext, false)
		{
			this.adSession = adSession;
			this.notifier = new GroupAssociationNotifier(subscriptionId, base.UserContext);
			this.notifier.RegisterWithPendingRequestNotifier();
		}

		internal override void HandleNotificationInternal(Notification notification, MapiNotificationsLogEvent logEvent, object context)
		{
			QueryNotification queryNotification = notification as QueryNotification;
			if (queryNotification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress, string>((long)this.GetHashCode(), "GroupAssociationNotificationHandler.HandleNotificationInternal: Received a null QueryNotification object for user {0} SubscriptionId: {1}", base.UserContext.PrimarySmtpAddress, base.SubscriptionId);
				logEvent.NullNotification = true;
				return;
			}
			GroupAssociationNotificationPayload payloadFromNotification = this.GetPayloadFromNotification(queryNotification);
			lock (base.SyncRoot)
			{
				this.notifier.AddGroupAssociationNotificationPayload(payloadFromNotification);
				this.notifier.PickupData();
			}
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
			lock (base.SyncRoot)
			{
				base.InitSubscription();
				if (base.MissedNotifications)
				{
					this.notifier.AddRefreshPayload();
					this.notifier.PickupData();
					base.MissedNotifications = false;
				}
			}
		}

		protected override void InitSubscriptionInternal()
		{
			if (!base.UserContext.MailboxSessionLockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling method UnseenItemNotificationHandler.InitSubscriptionInternal");
			}
			using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, DefaultFolderType.MailboxAssociation))
			{
				base.QueryResult = this.GetQueryResult(folder);
				base.QueryResult.GetRows(base.QueryResult.EstimatedRowCount);
				base.Subscription = Subscription.Create(base.QueryResult, new NotificationHandler(base.HandleNotification));
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "GroupAssociationNotificationHandler.InitSubscriptionInternal succeeded for subscription {0}", base.SubscriptionId);
		}

		private static T GetItemProperty<T>(QueryNotification notification, int index, T defaultValue)
		{
			if (index >= notification.Row.Length || notification.Row[index] == null || notification.Row[index] is PropertyError)
			{
				return defaultValue;
			}
			if (notification.Row[index] is T)
			{
				return (T)((object)notification.Row[index]);
			}
			return defaultValue;
		}

		private QueryResult GetQueryResult(Folder folder)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "GroupAssociationNotificationHandler.GetQueryResult for subscription {0}", base.SubscriptionId);
			return folder.ItemQuery(ItemQueryType.None, null, GroupAssociationNotificationHandler.MailboxAssociationSortBy, GroupAssociationNotificationHandler.MailboxAssociationQuerySubscriptionProperties);
		}

		private GroupAssociationNotificationPayload GetPayloadFromNotification(QueryNotification notification)
		{
			GroupAssociationNotificationPayload groupAssociationNotificationPayload = new GroupAssociationNotificationPayload();
			groupAssociationNotificationPayload.SubscriptionId = base.SubscriptionId;
			groupAssociationNotificationPayload.EventType = notification.EventType;
			groupAssociationNotificationPayload.Source = MailboxLocation.FromMailboxContext(base.UserContext);
			if (notification.EventType != QueryNotificationType.RowDeleted)
			{
				string itemProperty = GroupAssociationNotificationHandler.GetItemProperty<string>(notification, 0, string.Empty);
				string itemProperty2 = GroupAssociationNotificationHandler.GetItemProperty<string>(notification, 1, string.Empty);
				bool itemProperty3 = GroupAssociationNotificationHandler.GetItemProperty<bool>(notification, 2, false);
				GroupMailboxLocator groupMailboxLocator = new GroupMailboxLocator(this.adSession, itemProperty, itemProperty2);
				ADUser aduser = groupMailboxLocator.FindAdUser();
				if (aduser != null)
				{
					groupAssociationNotificationPayload.Group = new ModernGroupType
					{
						DisplayName = aduser.DisplayName,
						SmtpAddress = aduser.PrimarySmtpAddress.ToString(),
						IsPinned = itemProperty3
					};
				}
				else
				{
					ExTraceGlobals.NotificationsCallTracer.TraceError<string, string>((long)this.GetHashCode(), "GroupAssociationNotificationHandler.GetPayloadFromNotification: Could not find Group in AD with ExternalObjectId {0} or LegacyDn {1}", itemProperty, itemProperty2);
				}
			}
			return groupAssociationNotificationPayload;
		}

		private static readonly PropertyDefinition[] MailboxAssociationQuerySubscriptionProperties = new PropertyDefinition[]
		{
			MailboxAssociationBaseSchema.ExternalId,
			MailboxAssociationBaseSchema.LegacyDN,
			MailboxAssociationBaseSchema.IsPin
		};

		private static readonly SortBy[] MailboxAssociationSortBy = new SortBy[]
		{
			new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
		};

		private readonly IRecipientSession adSession;

		protected readonly GroupAssociationNotifier notifier;
	}
}
