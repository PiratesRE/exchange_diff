using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecommendedGroupsAccessorFactory
	{
		public IRecommendedGroupsGetter GetReadOnlyAccessor()
		{
			return new RecommendedGroupsAccessor();
		}

		public IRecommendedGroupsAccessor GetReadWriteAccessor()
		{
			return new RecommendedGroupsAccessor();
		}
	}
}
