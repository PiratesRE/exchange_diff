using System;
using System.Web;

namespace Microsoft.Exchange.HttpProxy.ProxyAssistant
{
	internal class ProxyAssistantDiagnostics : IProxyAssistantDiagnostics
	{
		public ProxyAssistantDiagnostics(HttpContextBase httpContext)
		{
			this.logger = RequestLogger.GetLogger(httpContext);
		}

		public void AddErrorInfo(object value)
		{
			this.logger.AppendErrorInfo("ProxyAssistant", value);
		}

		public void LogUnhandledException(Exception ex)
		{
			this.logger.LastChanceExceptionHandler(ex);
		}

		private RequestLogger logger;
	}
}
