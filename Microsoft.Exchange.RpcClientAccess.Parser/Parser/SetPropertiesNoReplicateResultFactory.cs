using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetPropertiesNoReplicateResultFactory : StandardResultFactory
	{
		internal SetPropertiesNoReplicateResultFactory() : base(RopId.SetPropertiesNoReplicate)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyProblem[] propertyProblems)
		{
			return new SuccessfulSetPropertiesResult(RopId.SetPropertiesNoReplicate, propertyProblems);
		}
	}
}
