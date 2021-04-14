using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedInAppConfig
	{
		public string AppId { get; private set; }

		public string AppSecret { get; private set; }

		public string ProfileEndpoint { get; private set; }

		public string ConnectionsEndpoint { get; private set; }

		public string RemoveAppEndpoint { get; private set; }

		public TimeSpan WebRequestTimeout { get; private set; }

		public IWebProxy WebProxy { get; private set; }

		public LinkedInAppConfig(string appId, string appSecret, string profileEndpoint, string connectionsEndpoint, string removeAppEndpoint, TimeSpan webRequestTimeout, string webProxyUri)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			ArgumentValidator.ThrowIfNullOrEmpty("appSecret", appSecret);
			ArgumentValidator.ThrowIfNullOrEmpty("profileEndpoint", profileEndpoint);
			ArgumentValidator.ThrowIfNullOrEmpty("connectionsEndpoint", connectionsEndpoint);
			ArgumentValidator.ThrowIfNullOrEmpty("removeAppEndpoint", removeAppEndpoint);
			this.AppId = appId;
			this.AppSecret = appSecret;
			this.ProfileEndpoint = profileEndpoint;
			this.ConnectionsEndpoint = connectionsEndpoint;
			this.RemoveAppEndpoint = removeAppEndpoint;
			this.WebRequestTimeout = (TimeSpan.Zero.Equals(webRequestTimeout) ? LinkedInAppConfig.DefaultWebRequestTimeout : webRequestTimeout);
			this.WebProxy = (string.IsNullOrWhiteSpace(webProxyUri) ? null : new WebProxy(webProxyUri));
		}

		private static readonly TimeSpan DefaultWebRequestTimeout = TimeSpan.FromSeconds(10.0);
	}
}
