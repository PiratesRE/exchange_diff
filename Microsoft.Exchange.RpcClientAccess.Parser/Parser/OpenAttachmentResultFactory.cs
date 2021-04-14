using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OpenAttachmentResultFactory : StandardResultFactory
	{
		internal OpenAttachmentResultFactory() : base(RopId.OpenAttachment)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulOpenAttachmentResult(serverObject);
		}
	}
}
