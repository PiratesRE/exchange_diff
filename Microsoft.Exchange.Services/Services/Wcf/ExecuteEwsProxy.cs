using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class ExecuteEwsProxy : ServiceCommand<IAsyncResult>
	{
		static ExecuteEwsProxy()
		{
			CertificateValidationManager.RegisterCallback("EwsProxy", Global.RemoteCertificateValidationCallback);
		}

		public ExecuteEwsProxy(CallContext callContext, string body, string token, string extensionId, AsyncCallback asyncCallback, object asyncState) : base(callContext)
		{
			this.body = body;
			this.token = token;
			this.asyncCallback = asyncCallback;
			this.asyncState = asyncState;
			this.extensionId = extensionId;
		}

		protected override IAsyncResult InternalExecute()
		{
			this.asyncResult = new ServiceAsyncResult<EwsProxyResponse>();
			this.asyncResult.AsyncState = this.asyncState;
			this.asyncResult.AsyncCallback = this.asyncCallback;
			try
			{
				string text = (base.CallContext.IsOwa && VariantConfiguration.InvariantNoFlightingSnapshot.Ews.UseInternalEwsUrlForExtensionEwsProxyInOwa.Enabled) ? EwsHelper.DiscoverEwsUrl(base.CallContext.AccessingPrincipal) : EwsHelper.DiscoverExternalEwsUrl(base.CallContext.AccessingPrincipal);
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "[ExecuteEwsProxy::Execute] Original request url: {0}", text);
				Uri uri = new Uri(text);
				string text2 = string.Format("https://{0}/ews/exchange.asmx", uri.Host);
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "[ExecuteEwsProxy::Execute] Ews url: {0}", text2);
				Uri requestUri = new Uri(text2);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
				httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
				httpWebRequest.Headers.Add("Authorization", string.Format("Bearer {0}", this.token));
				httpWebRequest.PreAuthenticate = true;
				httpWebRequest.AllowAutoRedirect = false;
				httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
				httpWebRequest.ServerCertificateValidationCallback = Global.RemoteCertificateValidationCallback;
				CertificateValidationManager.SetComponentId(httpWebRequest, "EwsProxy");
				GccUtils.CopyClientIPEndpointsForServerToServerProxy(HttpContext.Current, httpWebRequest);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "text/xml; charset=utf-8";
				httpWebRequest.UserAgent = "EWSProxy/MailApp/" + this.extensionId;
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null)
				{
					currentActivityScope.SerializeTo(httpWebRequest);
				}
				byte[] bytes = Encoding.UTF8.GetBytes(this.body);
				httpWebRequest.ContentLength = (long)bytes.Length;
				httpWebRequest.BeginGetRequestStream(new AsyncCallback(this.GetRequestStreamCallback), httpWebRequest);
			}
			catch (WebException ex)
			{
				this.asyncResult.Data = ExecuteEwsProxy.GetEwsProxyResponseFromWebException(ex);
				this.asyncResult.Complete(ex);
			}
			catch (Exception ex2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, Exception>(0L, "[ExecuteEwsProxy::Execute] Exception occurred during EWS proxy for extension {0}: {1}", this.extensionId, ex2);
				this.asyncResult.Data = new EwsProxyResponse(ex2.Message);
				this.asyncResult.Complete(ex2);
			}
			return this.asyncResult;
		}

		private static EwsProxyResponse GetEwsProxyResponseFromWebException(WebException webException)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<WebException>(0L, "ExecuteEwsProxy.GetEwsProxyResponseFromWebException: Exception occurred during EWS proxy: {0}", webException);
			string errorMessage = webException.Message;
			HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				string faultString = ExecuteEwsProxy.GetFaultString(httpWebResponse);
				if (!string.IsNullOrEmpty(faultString))
				{
					errorMessage = faultString;
				}
			}
			return new EwsProxyResponse(errorMessage);
		}

		private static string GetFaultString(HttpWebResponse httpWebResponse)
		{
			string result = null;
			string text = null;
			try
			{
				text = ExecuteEwsProxy.GetResponseBody(httpWebResponse);
			}
			finally
			{
				if (httpWebResponse != null)
				{
					((IDisposable)httpWebResponse).Dispose();
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "ExecuteEwsProxy.GetFaultString: Response body is empty");
			}
			else
			{
				result = ExecuteEwsProxy.GetFaultString(text);
			}
			return result;
		}

		private static string GetFaultString(string responseBody)
		{
			string result = null;
			XDocument xdocument = null;
			try
			{
				xdocument = XDocument.Load(new StringReader(responseBody));
			}
			catch (XmlException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "ExecuteEwsProxy.GetFaultString: Response body is not xml, {0}", responseBody);
			}
			if (xdocument != null)
			{
				IEnumerable<XElement> source = xdocument.Descendants("faultstring");
				if (source.Count<XElement>() == 1)
				{
					result = source.First<XElement>().Value;
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "ExecuteEwsProxy.GetFaultString: faultString element is missing");
				}
			}
			return result;
		}

		private static string GetResponseBody(HttpWebResponse httpWebResponse)
		{
			string result = null;
			using (Stream responseStream = httpWebResponse.GetResponseStream())
			{
				using (StreamReader streamReader = new StreamReader(responseStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		private static EwsProxyResponse GetEwsProxyResponseFromWebResponse(HttpWebResponse httpWebResponse)
		{
			EwsProxyResponse result;
			try
			{
				string responseBody = ExecuteEwsProxy.GetResponseBody(httpWebResponse);
				if (responseBody != null && responseBody.Length > ExecuteEwsProxy.MaxEwsResponseSize)
				{
					result = new EwsProxyResponse(CoreResources.EwsProxyResponseTooBig);
				}
				else
				{
					result = new EwsProxyResponse((int)httpWebResponse.StatusCode, httpWebResponse.StatusDescription, responseBody);
				}
			}
			finally
			{
				if (httpWebResponse != null)
				{
					((IDisposable)httpWebResponse).Dispose();
				}
			}
			return result;
		}

		private void GetRequestStreamCallback(IAsyncResult result)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
			byte[] bytes = Encoding.UTF8.GetBytes(this.body);
			try
			{
				using (Stream stream = httpWebRequest.EndGetRequestStream(result))
				{
					stream.Write(bytes, 0, bytes.Length);
				}
				httpWebRequest.BeginGetResponse(new AsyncCallback(this.GetResponseCallback), httpWebRequest);
			}
			catch (WebException ex)
			{
				this.asyncResult.Data = new EwsProxyResponse(string.Format("Failed to make a request via HttpWebRequest. Exception: {0}", ex.Message));
				this.asyncResult.Complete(ex);
			}
			catch (IOException ex2)
			{
				this.asyncResult.Data = new EwsProxyResponse(string.Format("Failed to make a request via HttpWebRequest. Exception: {0}", ex2.Message));
				this.asyncResult.Complete(ex2);
			}
		}

		private void GetResponseCallback(IAsyncResult result)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(result);
				this.asyncResult.Data = ExecuteEwsProxy.GetEwsProxyResponseFromWebResponse(httpWebResponse);
				this.asyncResult.Complete(null);
			}
			catch (WebException ex)
			{
				this.asyncResult.Data = ExecuteEwsProxy.GetEwsProxyResponseFromWebException(ex);
				this.asyncResult.Complete(ex);
			}
		}

		private const string EwsUrlFormat = "https://{0}/ews/exchange.asmx";

		private static readonly int MaxEwsResponseSize = 1000000;

		private readonly string body;

		private readonly string token;

		private readonly string extensionId;

		private readonly AsyncCallback asyncCallback;

		private readonly object asyncState;

		private ServiceAsyncResult<EwsProxyResponse> asyncResult;
	}
}
