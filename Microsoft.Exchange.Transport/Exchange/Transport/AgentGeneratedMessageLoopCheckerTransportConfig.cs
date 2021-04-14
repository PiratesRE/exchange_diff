using System;

namespace Microsoft.Exchange.Transport
{
	internal class AgentGeneratedMessageLoopCheckerTransportConfig : AgentGeneratedMessageLoopCheckerConfig
	{
		internal AgentGeneratedMessageLoopCheckerTransportConfig(ITransportConfiguration transportConfiguration)
		{
			this.transportConfiguration = transportConfiguration;
		}

		internal override bool GetIsEnabledInSubmission()
		{
			return this.transportConfiguration.TransportSettings.TransportSettings.AgentGeneratedMessageLoopDetectionInSubmissionEnabled;
		}

		internal override bool GetIsEnabledInSmtp()
		{
			return this.transportConfiguration.TransportSettings.TransportSettings.AgentGeneratedMessageLoopDetectionInSmtpEnabled;
		}

		internal override uint GetMaxAllowedMessageDepth()
		{
			return this.transportConfiguration.TransportSettings.TransportSettings.MaxAllowedAgentGeneratedMessageDepth;
		}

		internal override uint GetMaxAllowedMessageDepthPerAgent()
		{
			return this.transportConfiguration.TransportSettings.TransportSettings.MaxAllowedAgentGeneratedMessageDepthPerAgent;
		}

		private readonly ITransportConfiguration transportConfiguration;
	}
}
