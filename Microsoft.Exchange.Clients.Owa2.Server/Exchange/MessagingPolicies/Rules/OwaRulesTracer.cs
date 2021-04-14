using System;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class OwaRulesTracer : ITracer
	{
		internal OwaRulesTracer()
		{
		}

		public void TraceDebug(string message)
		{
			ExTraceGlobals.OwaRulesEngineTracer.TraceDebug(0L, message);
		}

		public void TraceDebug(string formatString, params object[] args)
		{
			this.TraceDebug(string.Format(formatString, args));
		}

		public void TraceWarning(string message)
		{
			ExTraceGlobals.OwaRulesEngineTracer.TraceWarning(0L, message);
		}

		public void TraceWarning(string formatString, params object[] args)
		{
			this.TraceWarning(string.Format(formatString, args));
		}

		public void TraceError(string message)
		{
			ExTraceGlobals.OwaRulesEngineTracer.TraceError(0L, message);
		}

		public void TraceError(string formatString, params object[] args)
		{
			this.TraceError(string.Format(formatString, args));
		}
	}
}
