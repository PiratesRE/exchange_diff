using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCloneStreamResult : RopResult
	{
		internal SuccessfulCloneStreamResult(IServerObject serverObject) : base(RopId.CloneStream, ErrorCode.None, serverObject)
		{
		}

		internal SuccessfulCloneStreamResult(Reader reader) : base(reader)
		{
		}

		internal static RopResult Parse(Reader reader)
		{
			return new SuccessfulCloneStreamResult(reader);
		}
	}
}
