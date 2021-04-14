using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeletePropertiesResultFactory : StandardResultFactory
	{
		internal DeletePropertiesResultFactory() : base(RopId.DeleteProperties)
		{
		}

		public RopResult CreateSuccessfulResult(PropertyProblem[] propertyProblems)
		{
			return new SuccessfulDeletePropertiesResult(propertyProblems);
		}
	}
}
