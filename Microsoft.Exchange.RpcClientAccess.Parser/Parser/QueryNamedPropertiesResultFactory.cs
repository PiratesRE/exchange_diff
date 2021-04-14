using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class QueryNamedPropertiesResultFactory : StandardResultFactory
	{
		internal QueryNamedPropertiesResultFactory() : base(RopId.QueryNamedProperties)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyId[] propertyIds, NamedProperty[] namedProperties)
		{
			return new SuccessfulQueryNamedPropertiesResult(propertyIds, namedProperties);
		}
	}
}
