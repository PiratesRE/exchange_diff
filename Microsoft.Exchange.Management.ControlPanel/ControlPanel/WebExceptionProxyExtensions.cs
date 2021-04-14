using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class WebExceptionProxyExtensions
	{
		public static bool IsProxyNeedIdentityError(this WebException exception)
		{
			HttpWebResponse httpWebResponse = exception.Response as HttpWebResponse;
			return httpWebResponse != null && httpWebResponse.StatusCode == (HttpStatusCode)441;
		}

		public static ExEventLog.EventTuple GetProxyEventLogTuple(this WebException exception)
		{
			switch (exception.GetTroubleshootingID())
			{
			case WebExceptionTroubleshootingID.TrustFailure:
				return EcpEventLogConstants.Tuple_ProxyErrorSslTrustFailure;
			case WebExceptionTroubleshootingID.ServiceUnavailable:
				return EcpEventLogConstants.Tuple_ProxyErrorServiceUnavailable;
			case WebExceptionTroubleshootingID.Unauthorized:
				return EcpEventLogConstants.Tuple_ProxyErrorUnauthorized;
			case WebExceptionTroubleshootingID.Forbidden:
				return EcpEventLogConstants.Tuple_ProxyErrorForbidden;
			}
			return EcpEventLogConstants.Tuple_ProxyRequestFailed;
		}
	}
}
