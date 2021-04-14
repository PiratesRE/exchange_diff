using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.ExchangeService;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SubscribeToCalendarChanges : SingleStepServiceCommand<SubscribeToCalendarChangesRequest, SubscribeToCalendarChangesResponseMessage>
	{
		public SubscribeToCalendarChanges(CallContext callContext, SubscribeToCalendarChangesRequest request, Action<CalendarChangeNotificationType> callback) : base(callContext, request)
		{
			this.callback = callback;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			this.responseMessage.Initialize(base.Result.Code, base.Result.Error);
			return this.responseMessage;
		}

		internal override ServiceResult<SubscribeToCalendarChangesResponseMessage> Execute()
		{
			ExchangeServiceNotificationHandler handler = ExchangeServiceNotificationHandler.GetHandler(base.CallContext);
			if (base.Request.ParentFolderId == null)
			{
				handler.RemoveSubscription(base.Request.SubscriptionId);
			}
			else
			{
				handler.AddSubscription(base.CallContext.MailboxIdentityPrincipal, base.CallContext, base.Request.SubscriptionId, delegate
				{
					ExchangeServiceCalendarSubscription exchangeServiceCalendarSubscription = new ExchangeServiceCalendarSubscription(this.Request.SubscriptionId);
					StoreObjectId folderId = IdConverter.EwsIdToStoreObjectIdGivenStoreObjectType(this.Request.ParentFolderId.BaseFolderId.GetId(), StoreObjectType.Folder);
					using (Folder folder = Folder.Bind(handler.Session, folderId))
					{
						exchangeServiceCalendarSubscription.QueryResult = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
						{
							ItemSchema.Id
						});
						exchangeServiceCalendarSubscription.QueryResult.GetRows(1);
						exchangeServiceCalendarSubscription.Callback = this.callback;
						exchangeServiceCalendarSubscription.Subscription = Subscription.Create(exchangeServiceCalendarSubscription.QueryResult, new NotificationHandler(exchangeServiceCalendarSubscription.HandleNotification));
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>(0L, "SubscribeToCalendarChanges.Execute: Adding subscription for {0}.", exchangeServiceCalendarSubscription.SubscriptionId);
					}
					return exchangeServiceCalendarSubscription;
				});
			}
			return new ServiceResult<SubscribeToCalendarChangesResponseMessage>(this.responseMessage);
		}

		private SubscribeToCalendarChangesResponseMessage responseMessage = new SubscribeToCalendarChangesResponseMessage();

		private readonly Action<CalendarChangeNotificationType> callback;
	}
}
