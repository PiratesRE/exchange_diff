using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders
{
	internal class ConversationResponseBuilder : ConversationDataResponseBuilderBase<IConversation, IConversation, ConversationResponseType, ConversationResponseType>
	{
		public ConversationResponseBuilder(IMailboxSession mailboxSession, IConversation conversation, IModernConversationNodeFactory conversationNodeFactory, IParticipantResolver resolver, ConversationNodeLoadingList loadingList, ConversationRequestArguments requestArguments) : base(mailboxSession, conversation, requestArguments, loadingList, conversationNodeFactory, resolver)
		{
		}

		protected override IEnumerable<Tuple<IConversation, ConversationResponseType>> XsoAndEwsConversationNodes
		{
			get
			{
				yield return new Tuple<IConversation, ConversationResponseType>(base.Conversation, base.Response);
				yield break;
			}
		}

		protected override ConversationResponseType BuildSkeleton()
		{
			return new ConversationResponseType();
		}

		protected override void BuildConversationProperties()
		{
			base.BuildConversationProperties();
			base.Response.CanDelete = EffectiveRightsProperty.GetFromEffectiveRights(base.Conversation.EffectiveRights, base.MailboxSession).Delete;
		}
	}
}
