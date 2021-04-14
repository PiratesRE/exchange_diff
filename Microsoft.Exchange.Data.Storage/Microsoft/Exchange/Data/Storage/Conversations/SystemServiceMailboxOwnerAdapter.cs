using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SystemServiceMailboxOwnerAdapter : IMailboxOwner
	{
		public bool SideConversationProcessingEnabled
		{
			get
			{
				return false;
			}
		}

		public bool ThreadedConversationProcessingEnabled
		{
			get
			{
				return false;
			}
		}

		public bool ModernConversationPreparationEnabled
		{
			get
			{
				return false;
			}
		}

		public bool SearchDuplicatedMessagesEnabled
		{
			get
			{
				return false;
			}
		}

		public bool IsGroupMailbox
		{
			get
			{
				return false;
			}
		}

		public bool SentToMySelf(ICorePropertyBag item)
		{
			return false;
		}

		public bool RequestExtraPropertiesWhenSearching
		{
			get
			{
				return false;
			}
		}
	}
}
