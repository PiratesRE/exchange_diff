using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransportNewMailResultFactory : StandardResultFactory
	{
		internal TransportNewMailResultFactory() : base(RopId.TransportNewMail)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.TransportNewMail, ErrorCode.None);
		}
	}
}
