using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class QueryColumnsAllResultFactory : StandardResultFactory
	{
		internal QueryColumnsAllResultFactory() : base(RopId.QueryColumnsAll)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyTag[] propertyTags)
		{
			return new SuccessfulQueryColumnsAllResult(propertyTags);
		}
	}
}
