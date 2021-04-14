using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EchoBinaryResultFactory : StandardResultFactory
	{
		internal EchoBinaryResultFactory() : base(RopId.EchoBinary)
		{
		}

		public RopResult CreateSuccessfulResult(int returnValue, byte[] outParameter)
		{
			return new SuccessfulEchoBinaryResult(returnValue, outParameter);
		}
	}
}
