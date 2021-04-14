using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal interface IMExSession : IExecutionControl
	{
		string Name { get; }

		long TotalProcessorTime { get; }

		IDispatcher Dispatcher { get; }

		AgentRecord CurrentAgent { get; }

		string LastAgentName { get; }

		string EventTopic { get; }

		void StartCpuTracking(string agentName);

		void StopCpuTracking();

		void CleanupCpuTracker();

		void Invoke(string topic, object source, object e);

		IAsyncResult BeginInvoke(string topic, object source, object e, AsyncCallback callback, object callbackState);

		void EndInvoke(IAsyncResult asyncResult);

		object Clone();

		void Close();

		void Dispose();

		DisposeTracker GetDisposeTracker();

		void SuppressDisposeTracker();
	}
}
