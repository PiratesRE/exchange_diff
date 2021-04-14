using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SubscribeToHierarchyChanges : SingleStepServiceCommand<SubscribeToHierarchyChangesRequest, SubscribeToHierarchyChangesResponseMessage>
	{
		public SubscribeToHierarchyChanges(CallContext callContext, SubscribeToHierarchyChangesRequest request, Action<HierarchyNotification> callback) : base(callContext, request)
		{
			this.callback = callback;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			this.responseMessage.Initialize(base.Result.Code, base.Result.Error);
			return this.responseMessage;
		}

		internal override ServiceResult<SubscribeToHierarchyChangesResponseMessage> Execute()
		{
			ExchangeServiceNotificationHandler handler = ExchangeServiceNotificationHandler.GetHandler(base.CallContext);
			if (base.Request.MailboxGuid == Guid.Empty)
			{
				handler.RemoveSubscription(base.Request.SubscriptionId);
			}
			else
			{
				ExchangePrincipal exchangePrincipal = (base.Request.MailboxGuid == base.CallContext.MailboxIdentityPrincipal.MailboxInfo.MailboxGuid) ? base.CallContext.MailboxIdentityPrincipal : base.CallContext.MailboxIdentityPrincipal.GetAggregatedExchangePrincipal(base.Request.MailboxGuid);
				handler.AddSubscription(exchangePrincipal, base.CallContext, base.Request.SubscriptionId, delegate
				{
					ExchangeServiceHierarchySubscription exchangeServiceHierarchySubscription = new ExchangeServiceHierarchySubscription(this.Request.SubscriptionId);
					using (Folder folder = Folder.Bind(handler.Session, DefaultFolderType.Configuration))
					{
						exchangeServiceHierarchySubscription.QueryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, this.querySubscriptionProperties);
						exchangeServiceHierarchySubscription.QueryResult.GetRows(exchangeServiceHierarchySubscription.QueryResult.EstimatedRowCount);
						exchangeServiceHierarchySubscription.MailboxGuid = handler.Session.MailboxGuid;
						exchangeServiceHierarchySubscription.Callback = this.callback;
						exchangeServiceHierarchySubscription.Subscription = Subscription.Create(exchangeServiceHierarchySubscription.QueryResult, new NotificationHandler(exchangeServiceHierarchySubscription.HandleNotification));
					}
					return exchangeServiceHierarchySubscription;
				});
			}
			return new ServiceResult<SubscribeToHierarchyChangesResponseMessage>(this.responseMessage);
		}

		private SubscribeToHierarchyChangesResponseMessage responseMessage = new SubscribeToHierarchyChangesResponseMessage();

		private Action<HierarchyNotification> callback;

		private PropertyDefinition[] querySubscriptionProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.DisplayName,
			FolderSchema.ItemCount,
			FolderSchema.UnreadCount,
			StoreObjectSchema.ContainerClass,
			StoreObjectSchema.ParentItemId,
			FolderSchema.IPMFolder,
			FolderSchema.IsHidden
		};
	}
}
