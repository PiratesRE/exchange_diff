using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulTellVersionResult : RopResult
	{
		internal SuccessfulTellVersionResult() : base(RopId.TellVersion, ErrorCode.None, null)
		{
		}

		internal SuccessfulTellVersionResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulTellVersionResult Parse(Reader reader)
		{
			return new SuccessfulTellVersionResult(reader);
		}
	}
}
