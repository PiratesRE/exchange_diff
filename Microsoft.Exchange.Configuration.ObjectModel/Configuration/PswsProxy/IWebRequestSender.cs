using System;
using System.Collections.Specialized;
using System.Net;

namespace Microsoft.Exchange.Configuration.PswsProxy
{
	internal interface IWebRequestSender
	{
		WebResponse SendRequest(string requestUri, NetworkCredential credential, string method, int timeout, bool allowAutoRedirect, string contentType, NameValueCollection headers, string requestContent);
	}
}
