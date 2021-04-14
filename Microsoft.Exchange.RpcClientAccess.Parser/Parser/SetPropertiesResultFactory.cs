using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetPropertiesResultFactory : StandardResultFactory
	{
		internal SetPropertiesResultFactory() : base(RopId.SetProperties)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyProblem[] propertyProblems)
		{
			return new SuccessfulSetPropertiesResult(RopId.SetProperties, propertyProblems);
		}
	}
}
