using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public interface ITracer
	{
		void TraceDebug(string message);

		void TraceDebug(string formatString, params object[] args);

		void TraceWarning(string message);

		void TraceWarning(string formatString, params object[] args);

		void TraceError(string message);

		void TraceError(string formatString, params object[] args);
	}
}
