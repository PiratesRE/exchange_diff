using System;

namespace Microsoft.Exchange.Data.Transport
{
	internal interface IExecutionControl
	{
		string Id { get; }

		object CurrentEventSource { get; }

		object CurrentEventArgs { get; }

		string ExecutingAgentName { get; }

		string OutstandingEventTopic { get; }

		void HaltExecution();

		void OnStartAsyncAgent();

		void ResumeAgent();

		AgentAsyncCallback GetAgentAsyncCallback();
	}
}
