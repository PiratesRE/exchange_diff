using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SubscribeToConversationChanges : SingleStepServiceCommand<SubscribeToConversationChangesRequest, SubscribeToConversationChangesResponseMessage>
	{
		public SubscribeToConversationChanges(CallContext callContext, SubscribeToConversationChangesRequest request, Action<ConversationNotification> callback) : base(callContext, request)
		{
			this.callback = callback;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			this.responseMessage.Initialize(base.Result.Code, base.Result.Error);
			return this.responseMessage;
		}

		internal override ServiceResult<SubscribeToConversationChangesResponseMessage> Execute()
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				UnifiedView unifiedView = UnifiedView.Create(ExTraceGlobals.NotificationsCallTracer, base.CallContext, base.Request.MailboxGuids, base.Request.ParentFolderId);
				if (unifiedView != null)
				{
					disposeGuard.Add<UnifiedView>(unifiedView);
				}
				ExchangeServiceNotificationHandler handler = ExchangeServiceNotificationHandler.GetHandler(base.CallContext, unifiedView != null && unifiedView.UnifiedSessionRequired);
				if (base.Request.ParentFolderId == null || (unifiedView != null && !unifiedView.UnifiedViewScopeSpecified))
				{
					handler.RemoveSubscription(base.Request.SubscriptionId);
				}
				else
				{
					ExchangePrincipal exchangePrincipal = base.CallContext.MailboxIdentityPrincipal;
					if (unifiedView == null)
					{
						IdHeaderInformation idHeaderInformation = IdConverter.ConvertFromConcatenatedId(base.Request.ParentFolderId.BaseFolderId.GetId(), BasicTypes.Folder, null, false);
						Guid guid = Guid.Parse(idHeaderInformation.MailboxId.MailboxGuid);
						if (guid != base.CallContext.MailboxIdentityPrincipal.MailboxInfo.MailboxGuid)
						{
							exchangePrincipal = base.CallContext.MailboxIdentityPrincipal.GetAggregatedExchangePrincipal(guid);
						}
					}
					handler.AddSubscription(exchangePrincipal, base.CallContext, base.Request.SubscriptionId, delegate
					{
						ExchangeServiceConversationSubscription exchangeServiceConversationSubscription = new ExchangeServiceConversationSubscription(this.Request.SubscriptionId);
						IdAndSession idAndSession = (unifiedView == null) ? new IdAndSession(IdConverter.EwsIdToFolderId(this.Request.ParentFolderId.BaseFolderId.GetId()), handler.Session) : unifiedView.CreateIdAndSession(handler.Session);
						using (DisposeGuard disposeGuard2 = default(DisposeGuard))
						{
							Folder folder;
							if (unifiedView != null && unifiedView.SearchFolder != null)
							{
								folder = unifiedView.SearchFolder;
							}
							else
							{
								folder = Folder.Bind(idAndSession.Session, idAndSession.Id);
								disposeGuard2.Add<Folder>(folder);
							}
							SortBy[] sortColumns = SortResults.ToXsoSortBy(this.Request.SortOrder);
							ConversationResponseShape conversationResponseShape = this.Request.ConversationShape;
							if (conversationResponseShape == null)
							{
								conversationResponseShape = new ConversationResponseShape(ShapeEnum.Default, null);
							}
							if (unifiedView != null)
							{
								UnifiedView.UpdateConversationResponseShape(conversationResponseShape);
							}
							PropertyListForViewRowDeterminer propertyListForViewRowDeterminer = PropertyListForViewRowDeterminer.BuildForConversation(conversationResponseShape);
							exchangeServiceConversationSubscription.PropertyList = propertyListForViewRowDeterminer.GetPropertiesToFetch();
							exchangeServiceConversationSubscription.QueryResult = folder.ConversationItemQuery(null, sortColumns, exchangeServiceConversationSubscription.PropertyList);
							exchangeServiceConversationSubscription.QueryResult.GetRows(1);
							exchangeServiceConversationSubscription.MailboxGuid = handler.Session.MailboxGuid;
							exchangeServiceConversationSubscription.Callback = this.callback;
							exchangeServiceConversationSubscription.Subscription = Subscription.Create(exchangeServiceConversationSubscription.QueryResult, new NotificationHandler(exchangeServiceConversationSubscription.HandleNotification));
							ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>(0L, "SubscribeToConversationChanges.Execute: Adding subscription for {0}.", exchangeServiceConversationSubscription.SubscriptionId);
						}
						return exchangeServiceConversationSubscription;
					});
				}
			}
			return new ServiceResult<SubscribeToConversationChangesResponseMessage>(this.responseMessage);
		}

		private SubscribeToConversationChangesResponseMessage responseMessage = new SubscribeToConversationChangesResponseMessage();

		private Action<ConversationNotification> callback;
	}
}
