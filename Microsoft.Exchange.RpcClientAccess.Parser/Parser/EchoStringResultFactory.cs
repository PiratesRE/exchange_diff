using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EchoStringResultFactory : StandardResultFactory
	{
		internal EchoStringResultFactory() : base(RopId.EchoString)
		{
		}

		public RopResult CreateSuccessfulResult(string returnValue, string outParameter)
		{
			return new SuccessfulEchoStringResult(returnValue, outParameter);
		}
	}
}
