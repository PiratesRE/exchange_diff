using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CreateAttachmentResultFactory : StandardResultFactory
	{
		internal CreateAttachmentResultFactory() : base(RopId.CreateAttachment)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, uint attachmentNumber)
		{
			return new SuccessfulCreateAttachmentResult(serverObject, attachmentNumber);
		}
	}
}
