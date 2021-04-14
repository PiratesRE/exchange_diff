using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OpenCollectorResultFactory : StandardResultFactory
	{
		internal OpenCollectorResultFactory() : base(RopId.OpenCollector)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulOpenCollectorResult(serverObject);
		}
	}
}
