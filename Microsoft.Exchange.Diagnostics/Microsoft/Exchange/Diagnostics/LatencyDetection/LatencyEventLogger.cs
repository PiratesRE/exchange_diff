using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LatencyEventLogger : ILatencyDetectionLogger
	{
		public LoggingType Type
		{
			get
			{
				return LoggingType.EventLog;
			}
		}

		public void Log(LatencyReportingThreshold threshold, LatencyDetectionContext trigger, ICollection<LatencyDetectionContext> context, LatencyDetectionException exception)
		{
			if (threshold == null)
			{
				throw new ArgumentNullException("threshold");
			}
			if (trigger == null)
			{
				throw new ArgumentNullException("trigger");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			LatencyEventLogger.eventLogger.LogEvent(CommonEventLogConstants.Tuple_LatencyDetection, string.Empty, new object[]
			{
				threshold.Threshold.TotalMilliseconds.ToString(),
				string.Format("Trigger:{0}", trigger.ToString("s"))
			});
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.CommonTracer.Category, "MSExchange Common");
	}
}
