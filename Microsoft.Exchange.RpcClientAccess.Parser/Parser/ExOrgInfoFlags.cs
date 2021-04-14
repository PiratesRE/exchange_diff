using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ExOrgInfoFlags : uint
	{
		None = 0U,
		PublicFoldersEnabled = 1U,
		UseAutoDiscoverForPublicFolderConfiguration = 2U
	}
}
