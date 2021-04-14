using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulOpenAttachmentResult : RopResult
	{
		internal SuccessfulOpenAttachmentResult(IServerObject serverObject) : base(RopId.OpenAttachment, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulOpenAttachmentResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulOpenAttachmentResult Parse(Reader reader)
		{
			return new SuccessfulOpenAttachmentResult(reader);
		}
	}
}
