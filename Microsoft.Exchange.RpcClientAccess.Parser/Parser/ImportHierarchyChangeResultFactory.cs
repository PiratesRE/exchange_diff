using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportHierarchyChangeResultFactory : StandardResultFactory
	{
		internal ImportHierarchyChangeResultFactory() : base(RopId.ImportHierarchyChange)
		{
		}

		public RopResult CreateSuccessfulResult(StoreId folderId)
		{
			return new SuccessfulImportHierarchyChangeResult(folderId);
		}
	}
}
