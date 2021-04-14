using System;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DisabledRequestLogger : RequestLogger
	{
		public override LatencyTracker LatencyTracker
		{
			get
			{
				return DisabledRequestLogger.disabledLatencyTracker;
			}
		}

		public override void LogField(LogKey key, object value)
		{
		}

		public override void AppendGenericInfo(string key, object value)
		{
		}

		public override void AppendErrorInfo(string key, object value)
		{
		}

		public override void LogExceptionDetails(string key, Exception ex)
		{
		}

		public override void Flush()
		{
		}

		private static LatencyTracker disabledLatencyTracker = new DisabledLatencyTracker();
	}
}
