using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Configuration.PswsProxy
{
	internal class HttpWebRequestSender : IWebRequestSender
	{
		public WebResponse SendRequest(string requestUri, NetworkCredential credential, string method, int timeout, bool allowAutoRedirect, string contentType, NameValueCollection headers, string requestContent)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.Credentials = credential;
			httpWebRequest.Method = method;
			httpWebRequest.Timeout = timeout;
			httpWebRequest.AllowAutoRedirect = allowAutoRedirect;
			httpWebRequest.ContentType = contentType;
			foreach (object obj in headers.Keys)
			{
				string name = (string)obj;
				httpWebRequest.Headers[name] = headers[name];
			}
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				currentActivityScope.SerializeTo(httpWebRequest);
			}
			if (!string.IsNullOrEmpty(requestContent))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(requestContent);
				try
				{
					using (Stream requestStream = httpWebRequest.GetRequestStream())
					{
						requestStream.Write(bytes, 0, bytes.Length);
					}
				}
				catch (Exception ex)
				{
					throw new PswsProxyException(Strings.PswsRequestException(ex.Message), ex);
				}
			}
			WebResponse result = null;
			try
			{
				result = httpWebRequest.GetResponse();
			}
			catch (Exception ex2)
			{
				throw new PswsProxyException(Strings.PswsRequestException(ex2.Message), ex2);
			}
			return result;
		}
	}
}
