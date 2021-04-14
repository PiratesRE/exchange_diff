using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FlushRecipientsResultFactory : StandardResultFactory
	{
		internal FlushRecipientsResultFactory() : base(RopId.FlushRecipients)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.FlushRecipients, ErrorCode.None);
		}
	}
}
