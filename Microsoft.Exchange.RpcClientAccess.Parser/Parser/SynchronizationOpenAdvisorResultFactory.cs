using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SynchronizationOpenAdvisorResultFactory : StandardResultFactory
	{
		internal SynchronizationOpenAdvisorResultFactory() : base(RopId.SynchronizationOpenAdvisor)
		{
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject)
		{
			return new SuccessfulSynchronizationOpenAdvisorResult(serverObject);
		}
	}
}
