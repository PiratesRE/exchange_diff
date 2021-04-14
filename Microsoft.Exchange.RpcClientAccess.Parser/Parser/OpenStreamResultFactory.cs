using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OpenStreamResultFactory : StandardResultFactory
	{
		internal OpenStreamResultFactory() : base(RopId.OpenStream)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, uint streamSize)
		{
			return new SuccessfulOpenStreamResult(serverObject, streamSize);
		}
	}
}
