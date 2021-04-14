using System;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal interface IDispatcher
	{
		event AgentInvokeStartHandler OnAgentInvokeStart;

		event AgentInvokeReturnsHandler OnAgentInvokeReturns;

		event AgentInvokeEndHandler OnAgentInvokeEnd;

		event AgentInvokeScheduledHandler OnAgentInvokeScheduled;

		event AgentInvokeResumedHandler OnAgentInvokeResumed;

		void Invoke(MExSession session);

		void AgentInvokeCompleted(MExSession session);

		void AgentInvokeScheduled(MExSession session);

		void AgentInvokeResumed(MExSession session);

		void Shutdown();

		bool HasHandler(string eventTopic);

		void SetCloneState(string eventTopic, int firstAgentIndex);

		int GetAgentIndex(AgentRecord agentEntry);
	}
}
