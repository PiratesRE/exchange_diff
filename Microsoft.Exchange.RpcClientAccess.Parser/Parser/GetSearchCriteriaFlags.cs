using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum GetSearchCriteriaFlags : byte
	{
		None = 0,
		Unicode = 1,
		Restriction = 2,
		FolderIds = 4
	}
}
