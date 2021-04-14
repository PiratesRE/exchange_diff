using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetSearchCriteriaResultFactory : StandardResultFactory
	{
		internal GetSearchCriteriaResultFactory(byte logonIndex) : base(RopId.GetSearchCriteria)
		{
			this.logonIndex = logonIndex;
		}

		public RopResult CreateSuccessfulResult(Restriction restriction, StoreId[] folderIds, SearchState searchState)
		{
			return new SuccessfulGetSearchCriteriaResult(restriction, this.logonIndex, folderIds, searchState);
		}

		private readonly byte logonIndex;
	}
}
