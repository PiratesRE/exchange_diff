using System;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	internal static class PolicyNudgeConfigurationFactory
	{
		internal static PolicyNudgeConfiguration Create()
		{
			return new PolicyNudgeConfiguration15();
		}
	}
}
