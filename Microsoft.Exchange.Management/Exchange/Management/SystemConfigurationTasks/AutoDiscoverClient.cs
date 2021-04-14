using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class AutoDiscoverClient
	{
		public AutoDiscoverClient(string componentId, Task.TaskVerboseLoggingDelegate verbose, CredentialsImpersonator credentialsImpersonator, string emailAddress, string url, bool reportErrors, params string[] optionalHeaders)
		{
			this.verboseDelegate = verbose;
			this.credentialsImpersonator = credentialsImpersonator;
			this.url = url;
			this.emailAddress = emailAddress;
			this.reportErrors = reportErrors;
			this.componentId = componentId;
			if (optionalHeaders.Length % 2 != 0)
			{
				throw new ArgumentException("optionalHeaders");
			}
			this.additionalHeaders = new Dictionary<string, string>();
			for (int i = 0; i < optionalHeaders.Length; i += 2)
			{
				this.additionalHeaders.Add(optionalHeaders[i], optionalHeaders[i + 1]);
			}
		}

		public AutoDiscoverResponseXML Invoke()
		{
			return this.ExecuteAndReportErrors<AutoDiscoverResponseXML>(delegate
			{
				AutoDiscoverResponseXML result = null;
				this.credentialsImpersonator.Impersonate(delegate(ICredentials credentials)
				{
					HttpWebRequest httpWebRequest = this.SendRequest(credentials);
					using (WebResponse response = httpWebRequest.GetResponse())
					{
						result = this.GetResponse(response);
					}
				});
				return result;
			});
		}

		public IAsyncResult BeginInvoke(AsyncCallback asyncCallback)
		{
			return this.ExecuteAndReportErrors<IAsyncResult>(delegate
			{
				IAsyncResult result = null;
				this.credentialsImpersonator.Impersonate(delegate(ICredentials credentials)
				{
					this.request = this.SendRequest(credentials);
					result = this.request.BeginGetResponse(asyncCallback, this);
				});
				return result;
			});
		}

		public static AutoDiscoverResponseXML EndInvoke(IAsyncResult asyncResult, out string url)
		{
			AutoDiscoverClient autoDiscoverClient = asyncResult.AsyncState as AutoDiscoverClient;
			return autoDiscoverClient.EndInvokeInternal(asyncResult, out url);
		}

		private AutoDiscoverResponseXML EndInvokeInternal(IAsyncResult asyncResult, out string url)
		{
			url = this.url;
			return this.ExecuteAndReportErrors<AutoDiscoverResponseXML>(delegate
			{
				AutoDiscoverResponseXML response;
				using (WebResponse webResponse = this.request.EndGetResponse(asyncResult))
				{
					response = this.GetResponse(webResponse);
				}
				return response;
			});
		}

		private T ExecuteAndReportErrors<T>(Func<T> func) where T : class
		{
			Exception ex = null;
			try
			{
				return func();
			}
			catch (WebException ex2)
			{
				ex = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				ex = ex3;
			}
			if (this.reportErrors)
			{
				while (ex != null)
				{
					this.TraceVerbose(Strings.TowsException(this.url, ex.Message));
					ex = ex.InnerException;
				}
			}
			return default(T);
		}

		internal static void AddOutlook14Cookie(HttpWebRequest request)
		{
			if (request == null || request.RequestUri == null || string.IsNullOrEmpty(request.RequestUri.Host))
			{
				return;
			}
			request.CookieContainer = new CookieContainer();
			Cookie cookie = new Cookie("OutlookSession", "\"{" + Guid.NewGuid().ToString().ToUpper() + "}\"");
			cookie.Domain = request.RequestUri.Host;
			request.CookieContainer.Add(cookie);
		}

		private HttpWebRequest SendRequest(ICredentials credentials)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.url);
			AutoDiscoverClient.AddOutlook14Cookie(httpWebRequest);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "text/xml; charset=utf-8";
			httpWebRequest.Credentials = credentials;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
			foreach (string text in this.additionalHeaders.Keys)
			{
				httpWebRequest.Headers.Add(text, this.additionalHeaders[text]);
			}
			httpWebRequest.UserAgent = string.Format("{0}/{1}/{2}", Environment.MachineName, this.componentId, this.emailAddress);
			CertificateValidationManager.SetComponentId(httpWebRequest, this.componentId);
			this.TraceHeaders(httpWebRequest.Headers);
			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(AutoDiscoverRequestXML));
				AutoDiscoverRequestXML o = AutoDiscoverRequestXML.NewRequest(this.emailAddress);
				safeXmlSerializer.Serialize(requestStream, o);
			}
			return httpWebRequest;
		}

		private AutoDiscoverResponseXML GetResponse(WebResponse response)
		{
			this.TraceHeaders(response.Headers);
			AutoDiscoverResponseXML result;
			using (Stream responseStream = response.GetResponseStream())
			{
				try
				{
					SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(AutoDiscoverResponseXML));
					AutoDiscoverResponseXML autoDiscoverResponseXML = safeXmlSerializer.Deserialize(responseStream) as AutoDiscoverResponseXML;
					if (this.url.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
					{
						this.TraceVerbose(Strings.TowsDomainNotSsl(this.url));
					}
					result = autoDiscoverResponseXML;
				}
				catch (InvalidOperationException ex)
				{
					if (ex.InnerException == null || !(ex.InnerException is XmlException))
					{
						throw;
					}
					result = null;
				}
			}
			return result;
		}

		private void TraceHeaders(WebHeaderCollection headers)
		{
			if (headers != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in headers.AllKeys)
				{
					stringBuilder.AppendFormat("{0}: {1}\n", text, headers[text]);
				}
				this.TraceVerbose(new LocalizedString(stringBuilder.ToString()));
			}
		}

		private void TraceVerbose(LocalizedString message)
		{
			if (this.verboseDelegate != null)
			{
				this.verboseDelegate(message);
			}
		}

		private readonly Task.TaskVerboseLoggingDelegate verboseDelegate;

		private CredentialsImpersonator credentialsImpersonator;

		private readonly string emailAddress;

		private readonly string url;

		private readonly bool reportErrors;

		private HttpWebRequest request;

		private readonly string componentId;

		private Dictionary<string, string> additionalHeaders;
	}
}
