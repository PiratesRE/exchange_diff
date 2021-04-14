using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal sealed class Logger : ILogger
	{
		public Logger(Trace trace)
		{
			ArgumentValidator.ThrowIfNull("trace", trace);
			this.trace = trace;
			this.traceId = this.GetHashCode();
		}

		public void LogEvent(ExEventLog.EventTuple eventTuple, Exception exception)
		{
			StoreDriverDeliveryDiagnostics.LogEvent(eventTuple, exception.Message, new object[]
			{
				exception
			});
		}

		public void TraceDebug(params string[] messagesStrings)
		{
			if (messagesStrings.Length > 0 && this.trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.trace.TraceDebug((long)this.traceId, string.Concat(messagesStrings));
			}
		}

		private readonly Trace trace;

		private readonly int traceId;
	}
}
