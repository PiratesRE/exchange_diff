using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class SoapAutoDiscoverRequest : AsyncWebRequest, IDisposable
	{
		public SoapAutoDiscoverRequest(Application application, ClientContext clientContext, RequestLogger requestLogger, string label, AutoDiscoverAuthenticator authenticator, Uri targetUri, EmailAddress[] emailAddresses, AutodiscoverType autodiscoverType) : base(application, clientContext, requestLogger, label)
		{
			this.authenticator = authenticator;
			this.TargetUri = SoapAutoDiscoverRequest.FixTargetUri(targetUri);
			this.EmailAddresses = emailAddresses;
			this.AutodiscoverType = autodiscoverType;
		}

		private static Uri FixTargetUri(Uri uri)
		{
			if (uri.AbsolutePath.EndsWith("/autodiscover.xml", StringComparison.OrdinalIgnoreCase))
			{
				return new UriBuilder(uri)
				{
					Path = "/autodiscover/autodiscover.svc"
				}.Uri;
			}
			return uri;
		}

		public EmailAddress[] EmailAddresses { get; private set; }

		public Exception Exception { get; private set; }

		public string FrontEndServerName { get; private set; }

		public string BackEndServerName { get; private set; }

		public AutoDiscoverRequestResult[] Results { get; private set; }

		public Uri TargetUri { get; private set; }

		public AutodiscoverType AutodiscoverType { get; private set; }

		public override void Abort()
		{
			base.Abort();
			if (this.client != null)
			{
				this.client.Abort();
			}
		}

		public void Dispose()
		{
			if (this.client != null)
			{
				this.client.Dispose();
			}
		}

		protected abstract IAsyncResult BeginGetSettings(AsyncCallback callback);

		protected abstract AutodiscoverResponse EndGetSettings(IAsyncResult asyncResult);

		protected abstract void HandleResponse(AutodiscoverResponse response);

		protected override bool IsImpersonating
		{
			get
			{
				return this.authenticator.ProxyAuthenticator != null && this.authenticator.ProxyAuthenticator.AuthenticatorType == AuthenticatorType.NetworkCredentials;
			}
		}

		protected override IAsyncResult BeginInvoke()
		{
			this.autoDiscoverTimer = Stopwatch.StartNew();
			UserAgent userAgent = new UserAgent("ASAutoDiscover", "CrossForest", "EmailDomain", null);
			this.client = new DefaultBinding_Autodiscover(Globals.CertificateValidationComponentId, new RemoteCertificateValidationCallback(CertificateErrorHandler.CertValidationCallback));
			this.client.Url = this.TargetUri.ToString();
			this.client.UserAgent = userAgent.ToString();
			this.client.RequestedServerVersionValue = SoapAutoDiscoverRequest.Exchange2010RequestedServerVersion;
			Server localServer = LocalServerCache.LocalServer;
			if (localServer != null && localServer.InternetWebProxy != null)
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceDebug<SoapAutoDiscoverRequest, Uri>((long)this.GetHashCode(), "{0}: Using custom InternetWebProxy {1}.", this, localServer.InternetWebProxy);
				this.client.Proxy = new WebProxy(localServer.InternetWebProxy);
			}
			this.authenticator.Authenticate(this.client);
			SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceDebug<object, SoapAutoDiscoverRequest>((long)this.GetHashCode(), "{0}: Requesting: {1}", TraceContext.Get(), this);
			IAsyncResult result = null;
			Exception ex = this.ExecuteAndHandleException(delegate
			{
				result = this.BeginGetSettings(new AsyncCallback(this.Complete));
			});
			if (ex != null)
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<object, SoapAutoDiscoverRequest, Exception>((long)this.GetHashCode(), "{0}: Request '{1}' failed due exception: {2}", TraceContext.Get(), this, ex);
				this.HandleException(ex);
				return null;
			}
			return result;
		}

		protected override bool ShouldCallBeginInvokeByNewThread
		{
			get
			{
				return this.authenticator.ProxyAuthenticator != null && this.authenticator.ProxyAuthenticator.AuthenticatorType == AuthenticatorType.OAuth;
			}
		}

		protected override void EndInvoke(IAsyncResult asyncResult)
		{
			AutodiscoverResponse response = null;
			Exception ex = this.ExecuteAndHandleException(delegate
			{
				response = this.EndGetSettings(asyncResult);
			});
			Dictionary<string, string> responseHttpHeaders = this.client.ResponseHttpHeaders;
			if (responseHttpHeaders.ContainsKey(WellKnownHeader.XFEServer))
			{
				this.FrontEndServerName = responseHttpHeaders[WellKnownHeader.XFEServer];
			}
			if (responseHttpHeaders.ContainsKey(WellKnownHeader.XBEServer))
			{
				this.BackEndServerName = responseHttpHeaders[WellKnownHeader.XBEServer];
			}
			if (ex != null)
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<object, SoapAutoDiscoverRequest, Exception>((long)this.GetHashCode(), "{0}: Request '{1}' failed due exception: {2}", TraceContext.Get(), this, ex);
				this.HandleException(ex);
				return;
			}
			if (response == null)
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<object, SoapAutoDiscoverRequest>((long)this.GetHashCode(), "{0}: Request '{1}' succeeded, but received empty response", TraceContext.Get(), this);
				this.HandleException(new AutoDiscoverFailedException(Strings.descSoapAutoDiscoverInvalidResponseError(this.client.Url), 59196U));
				return;
			}
			if (response.ErrorCodeSpecified && response.ErrorCode != ErrorCode.NoError)
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError((long)this.GetHashCode(), "{0}: Request '{1}' failed with error {2}:{3}", new object[]
				{
					TraceContext.Get(),
					this,
					response.ErrorCode,
					response.ErrorMessage
				});
				this.HandleException(new AutoDiscoverFailedException(Strings.descSoapAutoDiscoverResponseError(this.client.Url, response.ErrorMessage), 34620U));
				return;
			}
			this.HandleResponse(response);
		}

		protected AutoDiscoverRequestResult GetAutodiscoverResult(string urlValue, string versionValue, EmailAddress emailAddress)
		{
			if (string.IsNullOrEmpty(urlValue) || !Uri.IsWellFormedUriString(urlValue, UriKind.Absolute))
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError((long)this.GetHashCode(), "{0}: Request '{1}' got ExternalEwsUrl setting for user {2} has invalid value: {3}", new object[]
				{
					TraceContext.Get(),
					this,
					emailAddress.Address,
					urlValue
				});
				return null;
			}
			int serverVersion = Globals.E14Version;
			if (!string.IsNullOrEmpty(versionValue))
			{
				Exception ex = null;
				try
				{
					Version version = new Version(versionValue);
					serverVersion = version.ToInt();
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				catch (FormatException ex3)
				{
					ex = ex3;
				}
				catch (OverflowException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<object, Exception>((long)this.GetHashCode(), "{0}: Exception parsing version: {1}", TraceContext.Get(), ex);
				}
			}
			string url = urlValue;
			if (this.authenticator.ProxyAuthenticator != null && this.authenticator.ProxyAuthenticator.AuthenticatorType == AuthenticatorType.WSSecurity)
			{
				url = EwsWsSecurityUrl.Fix(url);
			}
			return new AutoDiscoverRequestResult(this.TargetUri, null, null, new WebServiceUri(url, null, UriSource.EmailDomain, serverVersion), null, null);
		}

		private Exception ExecuteAndHandleException(SoapAutoDiscoverRequest.ExecuteAndHandleExceptionDelegate operation)
		{
			Exception ex = null;
			try
			{
				operation();
				return null;
			}
			catch (LocalizedException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (WebException ex4)
			{
				ex = ex4;
			}
			catch (SoapException ex5)
			{
				ex = ex5;
			}
			catch (InvalidOperationException ex6)
			{
				ex = ex6;
			}
			SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<object, SoapAutoDiscoverRequest, Exception>((long)this.GetHashCode(), "{0}: Request '{1}' failed due exception: {2}", TraceContext.Get(), this, ex);
			return ex;
		}

		protected override void HandleException(Exception exception)
		{
			this.Exception = exception;
		}

		protected void HandleResult(AutoDiscoverRequestResult[] autoDiscoverResults)
		{
			this.autoDiscoverTimer.Stop();
			base.RequestLogger.Add(RequestStatistics.Create(RequestStatisticsType.AutoDiscoverRequest, this.autoDiscoverTimer.ElapsedMilliseconds, autoDiscoverResults.Length, this.TargetUri.ToString()));
			this.Results = autoDiscoverResults;
		}

		protected const string ExternalEwsUrl = "ExternalEwsUrl";

		protected const string InteropExternalEwsUrl = "InteropExternalEwsUrl";

		protected const string ExternalEwsVersion = "ExternalEwsVersion";

		protected const string InteropExternalEwsVersion = "InteropExternalEwsVersion";

		protected AutoDiscoverAuthenticator authenticator;

		private Stopwatch autoDiscoverTimer;

		protected DefaultBinding_Autodiscover client;

		private static readonly RequestedServerVersion Exchange2010RequestedServerVersion = new RequestedServerVersion
		{
			Text = new string[]
			{
				"Exchange2010"
			}
		};

		protected static readonly Microsoft.Exchange.Diagnostics.Trace AutoDiscoverTracer = ExTraceGlobals.AutoDiscoverTracer;

		private delegate void ExecuteAndHandleExceptionDelegate();
	}
}
