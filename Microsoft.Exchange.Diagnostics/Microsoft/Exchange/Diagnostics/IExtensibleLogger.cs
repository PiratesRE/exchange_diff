using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface IExtensibleLogger : IWorkloadLogger
	{
		void LogEvent(ILogEvent logEvent);

		void LogEvent(IEnumerable<ILogEvent> logEvents);
	}
}
