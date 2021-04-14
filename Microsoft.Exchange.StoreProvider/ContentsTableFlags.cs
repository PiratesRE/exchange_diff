using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ContentsTableFlags
	{
		None = 0,
		ShowSoftDeletes = 2,
		ShowConversations = 256,
		ShowConversationMembers = 512,
		RetrieveFromIndex = 1024,
		NoNotifications = 32,
		Associated = 64,
		DeferredErrors = 8,
		Unicode = -2147483648,
		DocumentIdView = 2048,
		ExpandedConversationView = 8192,
		PrereadExtendedProperties = 16384
	}
}
