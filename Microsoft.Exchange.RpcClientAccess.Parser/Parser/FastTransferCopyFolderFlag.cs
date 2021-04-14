using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum FastTransferCopyFolderFlag : byte
	{
		None = 0,
		Move = 1,
		CopySubFolders = 16
	}
}
