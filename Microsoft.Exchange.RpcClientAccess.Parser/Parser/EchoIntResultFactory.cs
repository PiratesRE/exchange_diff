using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EchoIntResultFactory : StandardResultFactory
	{
		internal EchoIntResultFactory() : base(RopId.EchoInt)
		{
		}

		public RopResult CreateSuccessfulResult(int returnValue, int outParameter)
		{
			return new SuccessfulEchoIntResult(returnValue, outParameter);
		}
	}
}
