using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportDeleteResultFactory : StandardResultFactory
	{
		internal ImportDeleteResultFactory() : base(RopId.ImportDelete)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.ImportDelete, ErrorCode.None);
		}
	}
}
