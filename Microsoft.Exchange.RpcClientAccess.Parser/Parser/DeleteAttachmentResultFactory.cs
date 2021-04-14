using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeleteAttachmentResultFactory : StandardResultFactory
	{
		internal DeleteAttachmentResultFactory() : base(RopId.DeleteAttachment)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.DeleteAttachment, ErrorCode.None);
		}
	}
}
