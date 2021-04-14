using System;
using System.Collections.Generic;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public interface IHttpRequest
	{
		string AcceptLanguage { get; set; }

		string UserAgent { get; set; }

		ProcessCookies ProcessCookies { get; set; }

		ServerResponse SendGetRequest(string uri, bool sslValidation, string componentId, bool allowRedirects, int timeout, string authenticationType, string authenticationUser, string authenticationPassword, Dictionary<string, string> properties);

		ServerResponse SendPostRequest(string uri, bool allowRedirects, bool getHiddenInputValues, ref PostData postData, string contentType, string formName = null, int timeout = 0);
	}
}
