using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExchangeServiceConversationSubscription : ExchangeServiceSubscription
	{
		internal ExchangeServiceConversationSubscription(string subscriptionId) : base(subscriptionId)
		{
		}

		internal Guid MailboxGuid { get; set; }

		internal Subscription Subscription { get; set; }

		internal Action<ConversationNotification> Callback { get; set; }

		internal QueryResult QueryResult { get; set; }

		internal PropertyDefinition[] PropertyList { get; set; }

		internal override void HandleNotification(Notification notification)
		{
			ConversationNotification conversationNotification = null;
			if (notification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Received a null notification for subscriptionId: {0}", base.SubscriptionId);
				return;
			}
			if (notification is ConnectionDroppedNotification)
			{
				conversationNotification = new ConversationNotification();
				conversationNotification.NotificationType = NotificationTypeType.Reload;
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Connection dropped, returning notification for reload");
			}
			else
			{
				QueryNotification queryNotification = notification as QueryNotification;
				if (queryNotification == null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceWarning<string>((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Received a notification of an unknown type for subscriptionId: {0}", base.SubscriptionId);
					return;
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Received a {0} notification for subscriptionId: {1}", queryNotification.EventType.ToString(), base.SubscriptionId);
				switch (queryNotification.EventType)
				{
				case QueryNotificationType.RowAdded:
				case QueryNotificationType.RowModified:
					conversationNotification = new ConversationNotification();
					conversationNotification.NotificationType = ((queryNotification.EventType == QueryNotificationType.RowAdded) ? NotificationTypeType.Create : NotificationTypeType.Update);
					conversationNotification.Conversation = this.GetConversationFromNotification(queryNotification);
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Calling notification callback for conversation: {0}", conversationNotification.Conversation.ConversationId.Id);
					goto IL_19B;
				case QueryNotificationType.RowDeleted:
					conversationNotification = new ConversationNotification();
					conversationNotification.NotificationType = NotificationTypeType.Delete;
					conversationNotification.Conversation = this.GetConversationFromNotification(queryNotification, queryNotification.PropertyDefinitions.ToArray<PropertyDefinition>());
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Notification for deletion");
					goto IL_19B;
				case QueryNotificationType.Reload:
					conversationNotification = new ConversationNotification();
					conversationNotification.NotificationType = NotificationTypeType.Reload;
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Notification for reload");
					goto IL_19B;
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Unknown notification event type");
			}
			IL_19B:
			if (conversationNotification != null)
			{
				this.Callback(conversationNotification);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "ExchangeServiceConversationSubscription.HandleNotification: Returned from callback");
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
			return DisposeTracker.Get<ExchangeServiceConversationSubscription>(this);
		}

		private ConversationType GetConversationFromNotification(QueryNotification notification)
		{
			return this.GetConversationFromNotification(notification, this.PropertyList);
		}

		private ConversationType GetConversationFromNotification(QueryNotification notification, PropertyDefinition[] propertyList)
		{
			ConversationType conversationType = new ConversationType();
			conversationType.InstanceKey = notification.Index;
			conversationType.BulkAssignProperties(propertyList, notification.Row, this.MailboxGuid, null);
			return conversationType;
		}
	}
}
