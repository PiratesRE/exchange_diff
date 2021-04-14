using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Conversations;
using Microsoft.Exchange.Services.Core.Conversations.LoadingListBuilders;
using Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class GetConversationItemsDiagnostics : GetConversationItemsBase<IConversation, GetConversationItemsDiagnosticsRequest, GetConversationItemsDiagnosticsResponseType>
	{
		public GetConversationItemsDiagnostics(CallContext callContext, GetConversationItemsDiagnosticsRequest request) : base(callContext, request)
		{
		}

		protected override PropertyDefinition[] AdditionalRequestedProperties
		{
			get
			{
				return new PropertyDefinition[]
				{
					ItemSchema.InternetReferences,
					ItemSchema.ConversationIndexTrackingEx,
					ItemSchema.InReplyTo
				};
			}
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
			GetConversationItemsDiagnosticsResponse getConversationItemsDiagnosticsResponse = new GetConversationItemsDiagnosticsResponse();
			getConversationItemsDiagnosticsResponse.BuildForResults<GetConversationItemsDiagnosticsResponseType>(this.InternalResults);
			return getConversationItemsDiagnosticsResponse;
		}

		protected override ICoreConversationFactory<IConversation> CreateConversationFactory(IMailboxSession mailboxSession)
		{
			return new ConversationFactory((MailboxSession)mailboxSession);
		}

		protected override ConversationNodeLoadingListBuilderBase CreateConversationNodeLoadingListBuilder(IConversation conversation, ConversationRequestArguments requestArguments, List<IConversationTreeNode> nonSyncedNodes)
		{
			return new ConversationNodeDiagnosticsLoadingListBuilder(nonSyncedNodes);
		}

		protected override ConversationResponseBuilderBase<GetConversationItemsDiagnosticsResponseType> CreateBuilder(IMailboxSession mailboxSession, IConversation conversation, ConversationNodeLoadingList loadingList, ConversationRequestArguments requestArguments, ModernConversationNodeFactory conversationNodeFactory)
		{
			return new ConversationDiagnosticsResponseBuilder(mailboxSession, conversation, base.ParticipantResolver);
		}

		private const int DefaultMaxItemsToReturn = 100;
	}
}
