using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Conversations;
using Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders;
using Microsoft.Exchange.Services.Core.Conversations.Repositories;
using Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core
{
	internal class GetModernConversationItems : GetConversationItemsBase<IConversation, GetConversationItemsRequest, ConversationResponseType>
	{
		public GetModernConversationItems(CallContext callContext, GetConversationItemsRequest request) : base(callContext, request)
		{
		}

		protected override int MaxItemsToReturn
		{
			get
			{
				if (base.Request.MaxItemsToReturn == 0)
				{
					return 100;
				}
				return Math.Min(base.Request.MaxItemsToReturn, 100);
			}
		}

		protected override IExchangeWebMethodResponse GetResponseInternal()
		{
			GetConversationItemsResponse getConversationItemsResponse = new GetConversationItemsResponse();
			getConversationItemsResponse.BuildForResults<ConversationResponseType>(this.InternalResults);
			return getConversationItemsResponse;
		}

		protected override ICoreConversationFactory<IConversation> CreateConversationFactory(IMailboxSession mailboxSession)
		{
			ICoreConversationFactory<IConversation> result;
			if (base.CurrentConversationRequest.ConversationFamilyId == null)
			{
				result = new ConversationFactory(mailboxSession as MailboxSession);
			}
			else
			{
				ConversationId familyIdFromRequest = this.GetFamilyIdFromRequest();
				result = new ConversationFamilyFactory(mailboxSession, familyIdFromRequest);
			}
			return result;
		}

		protected override ConversationNodeLoadingListBuilderBase CreateConversationNodeLoadingListBuilder(IConversation conversation, ConversationRequestArguments requestArguments, List<IConversationTreeNode> nonSyncedNodes)
		{
			return new ConversationNodeLoadingListBuilder(nonSyncedNodes, conversation.FirstNode, requestArguments);
		}

		protected override ConversationResponseBuilderBase<ConversationResponseType> CreateBuilder(IMailboxSession mailboxSession, IConversation conversation, ConversationNodeLoadingList loadingList, ConversationRequestArguments requestArguments, ModernConversationNodeFactory conversationNodeFactory)
		{
			return new ConversationResponseBuilder(mailboxSession, conversation, conversationNodeFactory, base.ParticipantResolver, loadingList, requestArguments);
		}

		protected virtual ConversationId GetFamilyIdFromRequest()
		{
			IdAndSession sessionFromConversationId = XsoConversationRepositoryExtensions.GetSessionFromConversationId(base.IdConverter, base.CurrentConversationRequest.ConversationFamilyId, MailboxSearchLocation.PrimaryOnly);
			return sessionFromConversationId.Id as ConversationId;
		}

		private const int DefaultMaxItemsToReturn = 100;
	}
}
