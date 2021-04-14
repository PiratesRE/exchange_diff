using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetRulesTableResult : RopResult
	{
		internal SuccessfulGetRulesTableResult(IServerObject serverObject) : base(RopId.GetRulesTable, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulGetRulesTableResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulGetRulesTableResult Parse(Reader reader)
		{
			return new SuccessfulGetRulesTableResult(reader);
		}
	}
}
