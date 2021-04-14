using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders;
using Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core.Conversations
{
	internal class GetThreadedConversationItems : GetConversationItemsBase<IThreadedConversation, GetThreadedConversationItemsRequest, ThreadedConversationResponseType>
	{
		public GetThreadedConversationItems(CallContext callContext, GetThreadedConversationItemsRequest request) : base(callContext, request)
		{
		}

		protected override int MaxItemsToReturn
		{
			get
			{
				return 100;
			}
		}

		protected override IExchangeWebMethodResponse GetResponseInternal()
		{
			GetThreadedConversationItemsResponse getThreadedConversationItemsResponse = new GetThreadedConversationItemsResponse();
			getThreadedConversationItemsResponse.BuildForResults<ThreadedConversationResponseType>(this.InternalResults);
			return getThreadedConversationItemsResponse;
		}

		protected override ICoreConversationFactory<IThreadedConversation> CreateConversationFactory(IMailboxSession mailboxSession)
		{
			return new ThreadedConversationFactory(mailboxSession);
		}

		protected override ConversationNodeLoadingListBuilderBase CreateConversationNodeLoadingListBuilder(IThreadedConversation conversation, ConversationRequestArguments requestArguments, List<IConversationTreeNode> nonSyncedNodes)
		{
			return new ThreadedConversationNodeLoadingListBuilder(nonSyncedNodes, requestArguments);
		}

		protected override ConversationResponseBuilderBase<ThreadedConversationResponseType> CreateBuilder(IMailboxSession mailboxSession, IThreadedConversation conversation, ConversationNodeLoadingList loadingList, ConversationRequestArguments requestArguments, ModernConversationNodeFactory conversationNodeFactory)
		{
			return new ThreadedConversationResponseBuilder(mailboxSession, conversation, conversationNodeFactory, base.ParticipantResolver, loadingList, requestArguments);
		}

		private const int MaxItemsToReturnAcrossThreads = 100;
	}
}
