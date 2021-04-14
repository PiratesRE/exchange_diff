using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPropertyListResultFactory : StandardResultFactory
	{
		internal GetPropertyListResultFactory() : base(RopId.GetPropertyList)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyTag[] propertyTags)
		{
			return new SuccessfulGetPropertyListResult(propertyTags);
		}
	}
}
