using System;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRulesTracer : ITracer
	{
		internal ClientAccessRulesTracer(long traceId)
		{
			this.traceId = traceId;
		}

		public void TraceDebug(string message)
		{
			ExTraceGlobals.ClientAccessRulesTracer.TraceDebug(this.traceId, message);
		}

		public void TraceDebug(string formatString, params object[] args)
		{
			this.TraceDebug(string.Format(formatString, args));
		}

		public void TraceWarning(string message)
		{
			ExTraceGlobals.ClientAccessRulesTracer.TraceWarning(this.traceId, message);
		}

		public void TraceWarning(string formatString, params object[] args)
		{
			this.TraceWarning(string.Format(formatString, args));
		}

		public void TraceError(string message)
		{
			ExTraceGlobals.ClientAccessRulesTracer.TraceError(this.traceId, message);
		}

		public void TraceError(string formatString, params object[] args)
		{
			this.TraceError(string.Format(formatString, args));
		}

		private readonly long traceId;
	}
}
