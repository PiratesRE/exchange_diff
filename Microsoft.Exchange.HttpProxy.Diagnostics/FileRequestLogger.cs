using System;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	internal class FileRequestLogger : RequestLogger
	{
		public FileRequestLogger(HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			this.logData = httpContext.GetLogData();
			this.latencyTracker = new LatencyTracker(this.logData);
		}

		public override LatencyTracker LatencyTracker
		{
			get
			{
				return this.latencyTracker;
			}
		}

		public override void LogField(LogKey key, object value)
		{
			this.logData[key] = value;
		}

		public override void AppendGenericInfo(string key, object value)
		{
			this.logData.AppendGenericInfo(key, value);
		}

		public override void AppendErrorInfo(string key, object value)
		{
			this.logData.AppendErrorInfo(key, value);
		}

		public override void LogExceptionDetails(string key, Exception ex)
		{
			if (ex != null)
			{
				string value = ex.ToString();
				this.AppendErrorInfo(key, value);
			}
		}

		public override void Flush()
		{
			RequestLogListener.AppendLog(this.logData);
		}

		internal LogData ExposeLogDataForTesting()
		{
			return this.logData;
		}

		private LatencyTracker latencyTracker;

		private LogData logData;
	}
}
