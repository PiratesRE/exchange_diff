using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CloneStreamResultFactory : StandardResultFactory
	{
		internal CloneStreamResultFactory() : base(RopId.CloneStream)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulCloneStreamResult(serverObject);
		}
	}
}
