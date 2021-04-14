using System;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal interface ISmtpAgentSession
	{
		IAsyncResult BeginNoEvent(AsyncCallback callback, object state);

		IAsyncResult BeginRaiseEvent(string eventTopic, object eventSource, object eventArgs, AsyncCallback callback, object state);

		SmtpResponse EndRaiseEvent(IAsyncResult ar);

		Task<SmtpResponse> RaiseEventAsync(string eventTopic, object eventSource, object eventArgs);

		SmtpSession SessionSource { get; }

		AgentLatencyTracker LatencyTracker { get; }

		void Close();
	}
}
