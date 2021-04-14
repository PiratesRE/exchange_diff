using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupMailboxOwnerAdapter : IMailboxOwner
	{
		public bool SideConversationProcessingEnabled
		{
			get
			{
				return true;
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
				return true;
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
				return true;
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
				return true;
			}
		}
	}
}
