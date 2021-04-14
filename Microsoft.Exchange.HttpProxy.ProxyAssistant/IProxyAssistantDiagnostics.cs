using System;

namespace Microsoft.Exchange.HttpProxy.ProxyAssistant
{
	internal interface IProxyAssistantDiagnostics
	{
		void AddErrorInfo(object value);

		void LogUnhandledException(Exception ex);
	}
}
