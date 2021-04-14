using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Notifications.Broker;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class RowBrokerHandler : BrokerHandler
	{
		public RowBrokerHandler(string subscriptionId, SubscriptionParameters parameters, ExTimeZone timeZone, IMailboxContext userContext) : base(subscriptionId, parameters, userContext)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.rowNotifier = new RowNotifier(subscriptionId, userContext, userContext.ExchangePrincipal.MailboxInfo.MailboxGuid);
				this.rowNotifier.RegisterWithPendingRequestNotifier();
				this.folderId = StoreId.EwsIdToFolderStoreObjectId(parameters.FolderId);
				this.timeZone = timeZone;
				disposeGuard.Success();
			}
		}

		protected override BaseSubscription GetSubscriptionParmeters()
		{
			return new ConversationSubscription
			{
				ConsumerSubscriptionId = base.SubscriptionId,
				ClutterFilter = base.Parameters.ClutterFilter,
				ConversationShape = this.GetNotificationPayloadShape(base.Parameters.ConversationShapeName),
				CultureInfo = Thread.CurrentThread.CurrentCulture,
				Filter = base.Parameters.Filter,
				FromFilter = base.Parameters.FromFilter,
				FolderId = base.Parameters.FolderId,
				SortBy = base.Parameters.SortBy
			};
		}

		private ConversationResponseShape GetNotificationPayloadShape(string requestedConversationShapeName)
		{
			if (string.IsNullOrEmpty(requestedConversationShapeName))
			{
				requestedConversationShapeName = RowBrokerHandler.DefaultConversationViewShapeName;
			}
			ConversationResponseShape clientResponseShape = new ConversationResponseShape(ShapeEnum.IdOnly, Array<PropertyPath>.Empty);
			return Global.ResponseShapeResolver.GetResponseShape<ConversationResponseShape>(requestedConversationShapeName, clientResponseShape, this.GetFeaturesManager());
		}

		protected override void HandleNotificatonInternal(BrokerNotification notification)
		{
			Microsoft.Exchange.Notifications.Broker.ConversationNotification conversationNotification = notification.Payload as Microsoft.Exchange.Notifications.Broker.ConversationNotification;
			if (conversationNotification == null)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "[RowBrokerHandler.HandleNotificatonInternal]. Ignoring null/wrong type of payload.");
				return;
			}
			conversationNotification.Conversation.LastDeliveryTime = this.ConvertToRequestedTimeZone(conversationNotification.Conversation.LastDeliveryTime);
			conversationNotification.Conversation.LastDeliveryOrRenewTime = this.ConvertToRequestedTimeZone(conversationNotification.Conversation.LastDeliveryOrRenewTime);
			conversationNotification.Conversation.LastDeliveryOrRenewTime = this.ConvertToRequestedTimeZone(conversationNotification.Conversation.LastModifiedTime);
			this.rowNotifier.AddFolderContentChangePayload(this.folderId, new RowNotificationPayload
			{
				Conversation = conversationNotification.Conversation,
				EventType = conversationNotification.EventType,
				FolderId = conversationNotification.FolderId,
				Item = conversationNotification.Item,
				Prior = conversationNotification.Prior,
				SubscriptionId = conversationNotification.ConsumerSubscriptionId
			});
			this.rowNotifier.PickupData();
		}

		private string ConvertToRequestedTimeZone(string dateTimeStringInUtc)
		{
			string text = dateTimeStringInUtc;
			if (!string.IsNullOrEmpty(text) && this.timeZone != ExTimeZone.UtcTimeZone)
			{
				text = ExDateTimeConverter.ToOffsetXsdDateTime(ExDateTimeConverter.Parse(dateTimeStringInUtc), this.timeZone);
			}
			return text;
		}

		private IFeaturesManager GetFeaturesManager()
		{
			UserContext userContext = base.UserContext as UserContext;
			return (userContext == null) ? null : userContext.FeaturesManager;
		}

		private static readonly string DefaultConversationViewShapeName = WellKnownShapeName.ConversationUberListView.ToString();

		private readonly RowNotifier rowNotifier;

		private readonly StoreObjectId folderId;

		private readonly ExTimeZone timeZone;
	}
}
