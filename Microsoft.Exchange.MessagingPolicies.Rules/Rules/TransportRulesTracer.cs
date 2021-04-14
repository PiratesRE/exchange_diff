using System;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRulesTracer : ITracer
	{
		internal bool IsTestMessage { get; private set; }

		public override string ToString()
		{
			if (this.traceStringBuilder != null)
			{
				return this.traceStringBuilder.ToString();
			}
			return string.Empty;
		}

		internal TransportRulesTracer(MailItem mailItem, bool isTestMessage = false)
		{
			this.mailItem = mailItem;
			this.IsTestMessage = isTestMessage;
			if (this.IsTestMessage)
			{
				this.traceStringBuilder = new StringBuilder();
			}
		}

		public void TraceDebug(string message)
		{
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, message);
			if (this.mailItem != null)
			{
				SystemProbeHelper.EtrTracer.TracePass(this.mailItem, 0L, message);
			}
			if (this.traceStringBuilder != null)
			{
				this.traceStringBuilder.AppendLine(message);
			}
		}

		public void TraceDebug(string formatString, params object[] args)
		{
			this.TraceDebug(string.Format(formatString, args));
		}

		public void TraceWarning(string message)
		{
			ExTraceGlobals.TransportRulesEngineTracer.TraceWarning(0L, message);
			if (this.traceStringBuilder != null)
			{
				this.traceStringBuilder.AppendLine(message);
			}
		}

		public void TraceWarning(string formatString, params object[] args)
		{
			this.TraceWarning(string.Format(formatString, args));
		}

		public void TraceError(string message)
		{
			ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, message);
			if (this.traceStringBuilder != null)
			{
				this.traceStringBuilder.AppendLine(message);
			}
		}

		public void TraceError(string formatString, params object[] args)
		{
			this.TraceError(string.Format(formatString, args));
		}

		private MailItem mailItem;

		private StringBuilder traceStringBuilder;
	}
}
