using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetAttachmentTableResult : RopResult
	{
		internal SuccessfulGetAttachmentTableResult(IServerObject serverObject) : base(RopId.GetAttachmentTable, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulGetAttachmentTableResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulGetAttachmentTableResult Parse(Reader reader)
		{
			return new SuccessfulGetAttachmentTableResult(reader);
		}
	}
}
