using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class ProtocolPingStrategyBase
	{
		public virtual Uri BuildUrl(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			return new UriBuilder
			{
				Scheme = "https:",
				Host = fqdn,
				Path = HttpRuntime.AppDomainAppVirtualPath
			}.Uri;
		}

		public Exception Ping(Uri url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<Uri>((long)this.GetHashCode(), "[ProtocolPingStrategyBase::Ctor]: Testing server with URL {0}.", url);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ServicePoint.ConnectionLimit = HttpProxySettings.ServicePointConnectionLimit.Value;
			httpWebRequest.Method = "HEAD";
			httpWebRequest.Timeout = ProtocolPingStrategyBase.DownLevelServerPingTimeout.Value;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.UserAgent = "HttpProxy.ClientAccessServer2010Ping";
			httpWebRequest.KeepAlive = false;
			if (!HttpProxySettings.UseDefaultWebProxy.Value)
			{
				httpWebRequest.Proxy = NullWebProxy.Instance;
			}
			httpWebRequest.ServerCertificateValidationCallback = ProxyApplication.RemoteCertificateValidationCallback;
			CertificateValidationManager.SetComponentId(httpWebRequest, Constants.CertificateValidationComponentId);
			this.PrepareRequest(httpWebRequest);
			try
			{
				using (httpWebRequest.GetResponse())
				{
				}
			}
			catch (WebException ex)
			{
				ExTraceGlobals.VerboseTracer.TraceWarning<WebException>((long)this.GetHashCode(), "[ProtocolPingStrategyBase::TestServer]: Web exception: {0}.", ex);
				if (!this.IsWebExceptionExpected(ex))
				{
					return ex;
				}
			}
			catch (Exception ex2)
			{
				ExTraceGlobals.VerboseTracer.TraceError<Exception>((long)this.GetHashCode(), "[ProtocolPingStrategyBase::TestServer]: General exception {0}.", ex2);
				return ex2;
			}
			finally
			{
				try
				{
					httpWebRequest.Abort();
				}
				catch
				{
				}
			}
			return null;
		}

		protected virtual void PrepareRequest(HttpWebRequest request)
		{
		}

		protected virtual bool IsWebExceptionExpected(WebException exception)
		{
			return HttpWebHelper.CheckConnectivityError(exception) == HttpWebHelper.ConnectivityError.None;
		}

		private static readonly IntAppSettingsEntry DownLevelServerPingTimeout = new IntAppSettingsEntry(HttpProxySettings.Prefix("DownLevelServerPingTimeout"), 5000, ExTraceGlobals.VerboseTracer);
	}
}
