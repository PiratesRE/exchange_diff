using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class XmlHttpClient
	{
		public XmlHttpClient(Uri endpoint, WebProxy webProxy)
		{
			this.endpoint = endpoint;
			this.webProxy = webProxy;
		}

		public XmlDocument Invoke(XmlDocument requestXmlDocument)
		{
			HttpWebRequest httpWebRequest = this.SendRequest(requestXmlDocument);
			if (httpWebRequest == null)
			{
				throw new HttpClientException(this.endpoint);
			}
			XmlDocument result;
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					result = this.ReadXmlDocumentFromWebResponse(httpWebResponse);
				}
			}
			catch (WebException ex)
			{
				this.TraceResponseError(null, ex);
				throw new HttpClientException(this.endpoint, ex);
			}
			return result;
		}

		public IAsyncResult BeginInvoke(XmlDocument xmlDocument, AsyncCallback callback, object state)
		{
			HttpWebRequest httpWebRequest = this.SendRequest(xmlDocument);
			CustomContextAsyncResult customContextAsyncResult = new CustomContextAsyncResult(callback, state, httpWebRequest);
			customContextAsyncResult.InnerAsyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(customContextAsyncResult.CustomCallback), customContextAsyncResult);
			return customContextAsyncResult;
		}

		public XmlDocument EndInvoke(IAsyncResult asyncResult)
		{
			CustomContextAsyncResult customContextAsyncResult = (CustomContextAsyncResult)asyncResult;
			HttpWebRequest httpWebRequest = (HttpWebRequest)customContextAsyncResult.CustomState;
			XmlDocument result;
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(customContextAsyncResult.InnerAsyncResult))
				{
					result = this.ReadXmlDocumentFromWebResponse(httpWebResponse);
				}
			}
			catch (WebException ex)
			{
				this.TraceResponseError(null, ex);
				throw new HttpClientException(this.endpoint, ex);
			}
			return result;
		}

		public void AbortInvoke(IAsyncResult asyncResult)
		{
			CustomContextAsyncResult customContextAsyncResult = (CustomContextAsyncResult)asyncResult;
			HttpWebRequest httpWebRequest = (HttpWebRequest)customContextAsyncResult.CustomState;
			httpWebRequest.Abort();
		}

		private HttpWebRequest SendRequest(XmlDocument xmlDocument)
		{
			if (XmlHttpClient.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				XmlHttpClient.Tracer.TraceDebug<XmlHttpClient, string>((long)this.GetHashCode(), "{0}: Sending request: {1}", this, xmlDocument.OuterXml);
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.endpoint);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/soap+xml; charset=utf-8";
			httpWebRequest.ServicePoint.Expect100Continue = false;
			if (this.webProxy != null)
			{
				httpWebRequest.Proxy = this.webProxy;
			}
			try
			{
				using (Stream requestStream = httpWebRequest.GetRequestStream())
				{
					xmlDocument.Save(requestStream);
				}
			}
			catch (IOException ex)
			{
				this.TraceRequestError(ex);
				throw new HttpClientException(this.endpoint, ex);
			}
			catch (WebException ex2)
			{
				this.TraceRequestError(ex2);
				throw new HttpClientException(this.endpoint, ex2);
			}
			catch (XmlException ex3)
			{
				this.TraceRequestError(ex3);
				throw new HttpClientException(this.endpoint, ex3);
			}
			return httpWebRequest;
		}

		private XmlDocument ReadXmlDocumentFromWebResponse(HttpWebResponse response)
		{
			if (response.StatusCode != HttpStatusCode.OK)
			{
				this.TraceResponseError(response, null);
				throw new HttpClientException(this.endpoint);
			}
			XmlDocument result;
			try
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.Load(responseStream);
					if (XmlHttpClient.Tracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						XmlHttpClient.Tracer.TraceDebug<XmlHttpClient, string>((long)this.GetHashCode(), "{0}: Received response: {1}", this, xmlDocument.OuterXml);
					}
					result = xmlDocument;
				}
			}
			catch (ProtocolViolationException exception)
			{
				this.TraceResponseError(response, exception);
				throw new HttpClientException(this.endpoint);
			}
			catch (IOException exception2)
			{
				this.TraceResponseError(response, exception2);
				throw new HttpClientException(this.endpoint);
			}
			catch (XmlException exception3)
			{
				this.TraceResponseError(response, exception3);
				throw new HttpClientException(this.endpoint);
			}
			catch (WebException exception4)
			{
				this.TraceResponseError(response, exception4);
				throw new HttpClientException(this.endpoint);
			}
			return result;
		}

		private void TraceRequestError(Exception exception)
		{
			XmlHttpClient.Tracer.TraceError<XmlHttpClient, Exception>((long)this.GetHashCode(), "{0}: Failed request with: {1}", this, exception);
		}

		private void TraceResponseError(HttpWebResponse response, Exception exception)
		{
			if (XmlHttpClient.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (response != null)
				{
					stringBuilder.Append("StatusCode=");
					stringBuilder.Append(response.StatusCode);
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append("Headers=");
					stringBuilder.Append(response.Headers);
					stringBuilder.Append(Environment.NewLine);
				}
				if (exception == null && response != null)
				{
					try
					{
						using (Stream responseStream = response.GetResponseStream())
						{
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								stringBuilder.Append("Content=");
								stringBuilder.Append(streamReader.ReadToEnd());
								stringBuilder.Append(Environment.NewLine);
							}
						}
					}
					catch (ProtocolViolationException ex)
					{
						exception = ex;
					}
					catch (IOException ex2)
					{
						exception = ex2;
					}
					catch (WebException ex3)
					{
						exception = ex3;
					}
				}
				if (exception != null)
				{
					stringBuilder.Append("Exception=");
					while (exception != null)
					{
						stringBuilder.Append(exception.ToString());
						if (exception.InnerException != null)
						{
							stringBuilder.Append("InnerException=");
						}
						exception = exception.InnerException;
						stringBuilder.Append(Environment.NewLine);
					}
				}
				XmlHttpClient.Tracer.TraceError<XmlHttpClient, StringBuilder>((long)this.GetHashCode(), "{0}: Failed response with: {1}", this, stringBuilder);
			}
		}

		public override string ToString()
		{
			return "XmlHttpClient for " + this.endpoint.ToString();
		}

		private Uri endpoint;

		private WebProxy webProxy;

		private static readonly Trace Tracer = ExTraceGlobals.WSTrustTracer;
	}
}
