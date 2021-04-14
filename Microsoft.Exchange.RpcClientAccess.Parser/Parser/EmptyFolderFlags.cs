using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum EmptyFolderFlags : byte
	{
		None = 0,
		Associated = 1,
		Force = 2
	}
}
