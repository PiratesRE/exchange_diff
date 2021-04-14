using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IncrementalConfigResultFactory : StandardResultFactory
	{
		internal IncrementalConfigResultFactory() : base(RopId.IncrementalConfig)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject synchronizer)
		{
			return new SuccessfulIncrementalConfigResult(synchronizer);
		}
	}
}
