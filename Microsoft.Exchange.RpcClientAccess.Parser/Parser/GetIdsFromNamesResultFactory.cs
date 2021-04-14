using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetIdsFromNamesResultFactory : StandardResultFactory
	{
		internal GetIdsFromNamesResultFactory() : base(RopId.GetIdsFromNames)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyId[] propertyIds)
		{
			return new SuccessfulGetIdsFromNamesResult(propertyIds);
		}
	}
}
