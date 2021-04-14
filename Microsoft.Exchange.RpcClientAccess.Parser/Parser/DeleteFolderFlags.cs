using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum DeleteFolderFlags : byte
	{
		DeleteMessages = 1,
		DeleteFolders = 4,
		DeleteAssociated = 8,
		HardDelete = 16,
		Force = 32
	}
}
