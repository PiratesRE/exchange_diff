using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum ViewMessageConfigureFlags
	{
		None = 0,
		ViewFAI = 1,
		NoNotifications = 2,
		Conversation = 8,
		ConversationMembers = 16,
		SuppressNotifications = 128,
		ViewAll = 256,
		DoNotUseLazyIndex = 512,
		UseCoveringIndex = 1024,
		EmptyTable = 2048,
		MailboxScopeView = 4096,
		DocumentIdView = 4866,
		ExpandedConversations = 8192,
		PrereadExtendedProperties = 16384,
		RetrieveFromIndexOnly = 32768
	}
}
