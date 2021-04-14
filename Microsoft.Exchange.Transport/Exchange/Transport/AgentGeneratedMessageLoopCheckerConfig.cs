using System;

namespace Microsoft.Exchange.Transport
{
	internal abstract class AgentGeneratedMessageLoopCheckerConfig
	{
		internal abstract bool GetIsEnabledInSubmission();

		internal abstract bool GetIsEnabledInSmtp();

		internal abstract uint GetMaxAllowedMessageDepth();

		internal abstract uint GetMaxAllowedMessageDepthPerAgent();
	}
}
