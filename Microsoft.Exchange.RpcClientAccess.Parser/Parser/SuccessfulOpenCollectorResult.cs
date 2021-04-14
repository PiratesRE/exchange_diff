using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulOpenCollectorResult : RopResult
	{
		internal SuccessfulOpenCollectorResult(IServerObject serverObject) : base(RopId.OpenCollector, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulOpenCollectorResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulOpenCollectorResult Parse(Reader reader)
		{
			return new SuccessfulOpenCollectorResult(reader);
		}
	}
}
