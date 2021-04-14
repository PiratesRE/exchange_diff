using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSynchronizationOpenAdvisorResult : RopResult
	{
		internal SuccessfulSynchronizationOpenAdvisorResult(IServerObject serverObject) : base(RopId.SynchronizationOpenAdvisor, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
		}

		internal SuccessfulSynchronizationOpenAdvisorResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulSynchronizationOpenAdvisorResult Parse(Reader reader)
		{
			return new SuccessfulSynchronizationOpenAdvisorResult(reader);
		}
	}
}
