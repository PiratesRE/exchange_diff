using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ExtendedTableFlags
	{
		None = 0,
		RetrieveFromIndex = 1,
		Associated = 2,
		AclTableFreeBusy = 2,
		Depth = 4,
		ConversationView = 4,
		DeferredErrors = 8,
		NoNotifications = 16,
		SoftDeletes = 32,
		MapiUnicode = 64,
		SuppressNotifications = 128,
		ConversationViewMembers = 128,
		DocumentIdView = 256,
		ExpandedConversations = 512,
		PrereadExtendedProperties = 1024,
		MoreTableFlags = -2147483648
	}
}
