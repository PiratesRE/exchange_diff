using System;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal interface IRuntimeSettings
	{
		AgentRecord[] CreateDefaultAgentOrder();

		AgentRecord[] PublicAgentsInDefaultOrder { get; }

		void SaveAgentSubscription(AgentRecord[] agentRecords);

		bool DisposeAgents { get; }

		void AddSessionRef();

		void ReleaseSessionRef();

		FactoryTable AgentFactories { get; }

		MonitoringOptions MonitoringOptions { get; }
	}
}
