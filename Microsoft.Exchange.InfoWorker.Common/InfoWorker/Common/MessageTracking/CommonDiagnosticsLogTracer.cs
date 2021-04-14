using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class CommonDiagnosticsLogTracer : TraceWrapper.ITraceWriter
	{
		public void Write(string message)
		{
			KeyValuePair<string, object>[] eventData = new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("ThreadId", Environment.CurrentManagedThreadId),
				new KeyValuePair<string, object>("Message", message)
			};
			CommonDiagnosticsLog.Instance.LogEvent(CommonDiagnosticsLog.Source.Trace, eventData);
		}

		private const string ThreadId = "ThreadId";

		private const string Message = "Message";
	}
}
