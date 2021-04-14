using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ItemQueryType
	{
		None = 0,
		Associated = 1,
		SoftDeleted = 2,
		RetrieveFromIndex = 4,
		ConversationViewMembers = 8,
		ConversationView = 16,
		DocumentIdView = 32,
		PrereadExtendedProperties = 64,
		NoNotifications = 128
	}
}
