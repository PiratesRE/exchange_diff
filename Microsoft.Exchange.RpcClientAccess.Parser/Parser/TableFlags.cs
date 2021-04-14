using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum TableFlags : byte
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
		ConversationViewMembers = 128
	}
}
