using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TellVersionResultFactory : StandardResultFactory
	{
		internal TellVersionResultFactory() : base(RopId.TellVersion)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new SuccessfulTellVersionResult();
		}
	}
}
