using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.PushNotifications.Extensions;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class PushNotificationProxyRequest : HttpSessionConfig
	{
		public PushNotificationProxyRequest(ICredentials credentials = null, Uri targetUri = null) : this(credentials, targetUri, false)
		{
		}

		private PushNotificationProxyRequest(ICredentials credentials, Uri targetUri, bool isMonitoring)
		{
			base.Method = "POST";
			base.ContentType = "application/json";
			base.Pipelined = false;
			base.AllowAutoRedirect = false;
			base.KeepAlive = false;
			base.ReadWebExceptionResponseStream = true;
			base.UserAgent = PushNotificationProxyRequest.PushNotificationsUserAgent;
			this.Uri = targetUri;
			base.Headers = new WebHeaderCollection();
			if (isMonitoring)
			{
				WebHeaderCollection c = new WebHeaderCollection
				{
					{
						WellKnownHeader.Probe,
						WellKnownHeader.LocalProbeHeaderValue
					}
				};
				base.Headers.Add(c);
			}
			if (credentials != null)
			{
				base.Credentials = credentials;
				if (credentials is OAuthCredentials)
				{
					base.Headers[HttpRequestHeader.Authorization] = "Bearer";
					base.Headers["return-client-request-id"] = true.ToString();
					base.PreAuthenticate = true;
				}
			}
		}

		public string ClientRequestId
		{
			get
			{
				return base.Headers["client-request-id"];
			}
			set
			{
				base.Headers["client-request-id"] = value;
			}
		}

		public string ComponentId
		{
			get
			{
				return base.Headers[CertificateValidationManager.ComponentIdHeaderName];
			}
			set
			{
				base.Headers[CertificateValidationManager.ComponentIdHeaderName] = value;
			}
		}

		public string Authorization
		{
			get
			{
				return base.Headers[HttpRequestHeader.Authorization];
			}
			set
			{
				base.Headers[HttpRequestHeader.Authorization] = value;
			}
		}

		public Uri Uri { get; private set; }

		public string ToTraceString()
		{
			if (this.cachedToTraceString == null)
			{
				this.cachedToTraceString = string.Format("{{Request:'{0}'; Target-Uri:{1}; Method:{2}; Content-Type:{3}; Headers:[{4}]; ClientRequestId:{5}; ComponentId:{6};}}", new object[]
				{
					base.GetType().Name,
					this.Uri,
					base.Method,
					base.ContentType,
					base.Headers.ToTraceString(new string[]
					{
						HttpRequestHeader.Authorization.ToString()
					}),
					this.ClientRequestId,
					this.ComponentId
				});
			}
			return this.cachedToTraceString;
		}

		internal static PushNotificationProxyRequest CreateMonitoringRequest(ICredentials credentials)
		{
			return new PushNotificationProxyRequest(credentials, null, true);
		}

		private const string JsonContentType = "application/json";

		private const string AuthenticationBearerValue = "Bearer";

		private const string ReturnClientRequestIdHeader = "return-client-request-id";

		private static readonly string PushNotificationsUserAgent = string.Format("Exchange/{0}/PushNotificationsOnPremProxy", ExchangeSetupContext.InstalledVersion);

		private string cachedToTraceString;
	}
}
