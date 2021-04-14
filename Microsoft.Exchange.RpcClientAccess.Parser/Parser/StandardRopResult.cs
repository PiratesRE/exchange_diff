using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class StandardRopResult : RopResult
	{
		internal StandardRopResult(RopId ropId, ErrorCode errorCode) : base(ropId, errorCode, null)
		{
		}

		internal StandardRopResult(Reader reader) : base(reader)
		{
		}

		internal static RopResult ParseFailResult(Reader reader)
		{
			return new StandardRopResult(reader);
		}

		internal static RopResult ParseSuccessResult(Reader reader)
		{
			return new StandardRopResult(reader);
		}
	}
}
