using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SaveChangesAttachmentResultFactory : StandardResultFactory
	{
		internal SaveChangesAttachmentResultFactory() : base(RopId.SaveChangesAttachment)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SaveChangesAttachment, ErrorCode.None);
		}
	}
}
