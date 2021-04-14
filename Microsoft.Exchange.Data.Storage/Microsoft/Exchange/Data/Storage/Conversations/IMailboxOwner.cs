using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxOwner
	{
		bool SentToMySelf(ICorePropertyBag item);

		bool SideConversationProcessingEnabled { get; }

		bool ThreadedConversationProcessingEnabled { get; }

		bool ModernConversationPreparationEnabled { get; }

		bool SearchDuplicatedMessagesEnabled { get; }

		bool RequestExtraPropertiesWhenSearching { get; }

		bool IsGroupMailbox { get; }
	}
}
