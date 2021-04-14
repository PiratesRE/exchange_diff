using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal interface IRequestAdapter
	{
		TimeSpan RequestTimeout { get; set; }

		IResponseTracker ResponseTracker { get; set; }

		IAsyncResult BeginGetResponse(HttpWebRequestWrapper request, ExCookieContainer cookieContainer, SslValidationOptions sslValidationOptions, AuthenticationData? authenticationData, int maxRetryCount, AsyncCallback callback, Dictionary<string, object> asyncState);

		HttpWebResponseWrapper EndGetResponse(IAsyncResult result);

		void CloseConnections();
	}
}
