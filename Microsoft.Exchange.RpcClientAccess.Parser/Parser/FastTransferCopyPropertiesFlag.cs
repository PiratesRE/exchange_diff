using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum FastTransferCopyPropertiesFlag : byte
	{
		None = 0,
		Move = 1,
		FastTrasferStream = 2,
		CopyMailboxPerUserData = 8,
		CopyFolderPerUserData = 16
	}
}
