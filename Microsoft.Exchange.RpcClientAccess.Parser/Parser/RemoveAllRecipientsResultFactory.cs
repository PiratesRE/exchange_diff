using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemoveAllRecipientsResultFactory : StandardResultFactory
	{
		internal RemoveAllRecipientsResultFactory() : base(RopId.RemoveAllRecipients)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.RemoveAllRecipients, ErrorCode.None);
		}
	}
}
