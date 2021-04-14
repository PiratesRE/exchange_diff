using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AuthMetadataClient
	{
		static AuthMetadataClient()
		{
			CertificateValidationManager.RegisterCallback("Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthMetadataClient.NoSsl", new RemoteCertificateValidationCallback(AuthMetadataClient.ServerCertificateValidatorIgnoreSslErrors));
			CertificateValidationManager.RegisterCallback("Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthMetadataClient", new RemoteCertificateValidationCallback(AuthMetadataClient.ServerCertificateValidator));
		}

		private static bool ServerCertificateValidator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None;
		}

		private static bool ServerCertificateValidatorIgnoreSslErrors(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public AuthMetadataClient(string url, bool trustSslCert)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			this.Url = url;
			this.trustSslCert = trustSslCert;
			this.UserAgent = "MicrosoftExchangeAuthManagement";
		}

		public string Url { get; private set; }

		public string UserAgent { get; set; }

		public int Timeout { get; set; }

		public static AuthMetadata AcquireMetadata(string authMetadataUrl, bool requireIssuingEndpoint, bool trustSslCert, bool wrapException = true)
		{
			AuthMetadataClient authMetadataClient = new AuthMetadataClient(authMetadataUrl, trustSslCert);
			string content = authMetadataClient.Acquire(wrapException);
			AuthMetadata authMetadata;
			switch (AuthMetadataParser.DecideMetadataDocumentType(authMetadataUrl))
			{
			case AuthMetadataParser.MetadataDocType.OAuthS2SV1Metadata:
				return AuthMetadataParser.GetAuthMetadata(content, requireIssuingEndpoint);
			case AuthMetadataParser.MetadataDocType.WSFedMetadata:
				return AuthMetadataParser.GetWSFederationMetadata(content);
			case AuthMetadataParser.MetadataDocType.OAuthOpenIdConnectMetadata:
				authMetadata = AuthMetadataParser.GetOpenIdConnectAuthMetadata(content, requireIssuingEndpoint);
				if (!string.IsNullOrEmpty(authMetadata.KeysEndpoint))
				{
					authMetadataClient = new AuthMetadataClient(authMetadata.KeysEndpoint, trustSslCert);
					content = authMetadataClient.Acquire(wrapException);
					return AuthMetadataParser.GetOpenIdConnectKeys(content, authMetadata);
				}
				return authMetadata;
			}
			authMetadata = AuthMetadataParser.GetAuthMetadata(content, requireIssuingEndpoint);
			return authMetadata;
		}

		public string Acquire(bool wrapException = true)
		{
			if (wrapException)
			{
				try
				{
					return this.InternalAcquire();
				}
				catch (WebException ex)
				{
					WebResponse response = ex.Response;
					if (response != null)
					{
						if (response.Headers != null)
						{
							ExTraceGlobals.OAuthTracer.TraceError<WebHeaderCollection>((long)this.GetHashCode(), "[AuthMetadataClient:Acquire] response headers were {0}", response.Headers);
						}
						using (Stream responseStream = response.GetResponseStream())
						{
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								string arg = streamReader.ReadToEnd();
								ExTraceGlobals.OAuthTracer.TraceError<string>((long)this.GetHashCode(), "[AuthMetadataClient:Acquire] response content was {0}", arg);
							}
						}
					}
					throw new AuthMetadataClientException(DirectoryStrings.ErrorCannotAcquireAuthMetadata(this.Url, ex.Message), ex);
				}
			}
			return this.InternalAcquire();
		}

		private string InternalAcquire()
		{
			string result;
			using (AuthMetadataClient.TimeOutWebClient timeOutWebClient = new AuthMetadataClient.TimeOutWebClient(this.trustSslCert))
			{
				if (this.Timeout != 0)
				{
					timeOutWebClient.Timeout = this.Timeout;
				}
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && localServer.InternetWebProxy != null)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<Uri>((long)this.GetHashCode(), "[AuthMetadataClient:InternalAcquire] Using custom InternetWebProxy {0}", localServer.InternetWebProxy);
					timeOutWebClient.Proxy = new WebProxy(localServer.InternetWebProxy, true);
				}
				else
				{
					ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[AuthMetadataClient:InternalAcquire] Using null proxy");
					timeOutWebClient.Proxy = new WebProxy();
				}
				if (!string.IsNullOrEmpty(this.UserAgent))
				{
					timeOutWebClient.Headers.Add(HttpRequestHeader.UserAgent, this.UserAgent);
				}
				result = timeOutWebClient.DownloadString(this.Url);
			}
			return result;
		}

		public const string CertificateValidationComponentId = "Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthMetadataClient";

		public const string CertificateValidationComponentIdNoSsl = "Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthMetadataClient.NoSsl";

		public const string AuthMetadataClientUserAgent = "MicrosoftExchangeAuthManagement";

		private readonly bool trustSslCert;

		private class TimeOutWebClient : WebClient
		{
			public TimeOutWebClient(bool trustSslCert)
			{
				this.trustSslCert = trustSslCert;
			}

			public int Timeout { get; set; }

			protected override WebRequest GetWebRequest(Uri uri)
			{
				WebRequest webRequest = base.GetWebRequest(uri);
				if (webRequest is HttpWebRequest)
				{
					HttpWebRequest request = webRequest as HttpWebRequest;
					if (this.trustSslCert)
					{
						CertificateValidationManager.SetComponentId(request, "Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthMetadataClient.NoSsl");
					}
					else
					{
						CertificateValidationManager.SetComponentId(request, "Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthMetadataClient");
					}
				}
				if (this.Timeout != 0)
				{
					webRequest.Timeout = this.Timeout;
				}
				return webRequest;
			}

			private readonly bool trustSslCert;
		}
	}
}
