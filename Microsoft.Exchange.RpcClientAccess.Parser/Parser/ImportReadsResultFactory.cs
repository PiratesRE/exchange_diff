using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportReadsResultFactory : StandardResultFactory
	{
		internal ImportReadsResultFactory() : base(RopId.ImportReads)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.ImportReads, ErrorCode.None);
		}
	}
}
