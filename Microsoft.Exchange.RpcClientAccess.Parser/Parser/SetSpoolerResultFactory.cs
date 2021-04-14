using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetSpoolerResultFactory : StandardResultFactory
	{
		internal SetSpoolerResultFactory() : base(RopId.SetSpooler)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SetSpooler, ErrorCode.None);
		}
	}
}
