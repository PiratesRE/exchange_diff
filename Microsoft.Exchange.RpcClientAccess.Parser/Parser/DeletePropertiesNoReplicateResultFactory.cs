using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeletePropertiesNoReplicateResultFactory : StandardResultFactory
	{
		internal DeletePropertiesNoReplicateResultFactory() : base(RopId.DeletePropertiesNoReplicate)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyProblem[] propertyProblems)
		{
			return new SuccessfulDeletePropertiesNoReplicateResult(propertyProblems);
		}
	}
}
