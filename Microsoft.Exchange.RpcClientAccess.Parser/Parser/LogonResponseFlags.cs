using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum LogonResponseFlags : byte
	{
		None = 0,
		IsMailboxLocalized = 1,
		IsMailboxOwner = 2,
		HasSendAsRights = 4,
		IsOOF = 16
	}
}
