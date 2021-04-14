using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulIncrementalConfigResult : RopResult
	{
		internal SuccessfulIncrementalConfigResult(IServerObject synchronizer) : base(RopId.IncrementalConfig, ErrorCode.None, synchronizer)
		{
		}

		internal SuccessfulIncrementalConfigResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulIncrementalConfigResult Parse(Reader reader)
		{
			return new SuccessfulIncrementalConfigResult(reader);
		}
	}
}
