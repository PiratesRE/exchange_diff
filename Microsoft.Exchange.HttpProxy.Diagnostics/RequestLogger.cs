using System;
using System.Web;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	public abstract class RequestLogger
	{
		public abstract LatencyTracker LatencyTracker { get; }

		public static RequestLogger GetLogger(HttpContextBase httpContext)
		{
			if (HttpProxySettings.DiagnosticsEnabled.Value)
			{
				return new FileRequestLogger(httpContext);
			}
			return new DisabledRequestLogger();
		}

		public abstract void LogField(LogKey key, object value);

		public abstract void AppendGenericInfo(string key, object value);

		public abstract void AppendErrorInfo(string key, object value);

		public abstract void LogExceptionDetails(string key, Exception ex);

		public void LastChanceExceptionHandler(Exception ex)
		{
			this.LogExceptionDetails("Watson", ex);
			this.Flush();
		}

		public abstract void Flush();
	}
}
