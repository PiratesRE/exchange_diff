using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedInConfig
	{
		private LinkedInConfig(string appId, string appSecret, string requestTokenEndpoint, string accessTokenEndpoint, string connectionsEndpoint, TimeSpan webRequestTimeout, string webProxyUri, DateTime readTimeUtc)
		{
			this.AppId = appId;
			this.AppSecret = appSecret;
			this.RequestTokenEndpoint = requestTokenEndpoint;
			this.AccessTokenEndpoint = accessTokenEndpoint;
			this.ConnectionsEndpoint = connectionsEndpoint;
			this.WebRequestTimeout = (TimeSpan.Zero.Equals(webRequestTimeout) ? LinkedInConfig.DefaultWebRequestTimeout : webRequestTimeout);
			this.WebProxy = (string.IsNullOrWhiteSpace(webProxyUri) ? null : new WebProxy(webProxyUri));
			this.ReadTimeUtc = readTimeUtc;
		}

		public static LinkedInConfig CreateForAppAuth(string appId, string appSecret, string requestTokenEndpoint, string accessTokenEndpoint, TimeSpan webRequestTimeout, string webProxyUri, DateTime readTimeUtc)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			ArgumentValidator.ThrowIfNullOrEmpty("appSecret", appSecret);
			ArgumentValidator.ThrowIfNullOrEmpty("requestTokenEndpoint", requestTokenEndpoint);
			ArgumentValidator.ThrowIfNullOrEmpty("accessTokenEndpoint", accessTokenEndpoint);
			return new LinkedInConfig(appId, appSecret, requestTokenEndpoint, accessTokenEndpoint, string.Empty, webRequestTimeout, webProxyUri, readTimeUtc);
		}

		public string RequestTokenEndpoint { get; private set; }

		public string AccessTokenEndpoint { get; private set; }

		public string ConnectionsEndpoint { get; private set; }

		public string AppId { get; private set; }

		public string AppSecret { get; private set; }

		public IWebProxy WebProxy { get; private set; }

		public TimeSpan WebRequestTimeout { get; private set; }

		public DateTime ReadTimeUtc { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{{ App id: {0};  Access token endpoint: {1};  Request token endpoint: {2};  Connections endpoint: {3};  Web request timeout: {4};  App secret hash: {5:X8};  Read time: {6:u} }}", new object[]
			{
				this.AppId,
				this.AccessTokenEndpoint,
				this.RequestTokenEndpoint,
				this.ConnectionsEndpoint,
				this.WebRequestTimeout,
				this.GetAppSecretHashCode(),
				this.ReadTimeUtc
			});
		}

		private int GetAppSecretHashCode()
		{
			if (this.AppSecret == null)
			{
				return 0;
			}
			return this.AppSecret.GetHashCode();
		}

		private static readonly TimeSpan DefaultWebRequestTimeout = TimeSpan.FromSeconds(10.0);
	}
}
