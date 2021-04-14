using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FacebookAuthenticatorConfig
	{
		private FacebookAuthenticatorConfig()
		{
			this.Scope = FacebookAuthenticatorConfig.DefaultPermissions;
			this.WebRequestTimeout = FacebookAuthenticatorConfig.DefaultWebRequestTimeout;
		}

		public static FacebookAuthenticatorConfig CreateForAppAuthorization(string appId, string redirectUri, string authorizationEndpoint, CultureInfo locale, DateTime readTimeUtc)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			ArgumentValidator.ThrowIfNullOrEmpty("redirectUri", redirectUri);
			ArgumentValidator.ThrowIfNullOrEmpty("authorizationEndpoint", authorizationEndpoint);
			ArgumentValidator.ThrowIfNull("locale", locale);
			return new FacebookAuthenticatorConfig
			{
				AppId = appId,
				RedirectUri = redirectUri,
				AuthorizationEndpoint = authorizationEndpoint,
				Locale = FacebookAuthenticatorConfig.GetFacebookLocaleName(locale),
				ReadTimeUtc = readTimeUtc
			};
		}

		public static FacebookAuthenticatorConfig CreateForAppAuthentication(string appId, string appSecret, string redirectUri, string graphTokenEndpoint, IFacebookAuthenticationWebClient authClient, TimeSpan webRequestTimeout, DateTime readTimeUtc)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			ArgumentValidator.ThrowIfNullOrEmpty("appSecret", appSecret);
			ArgumentValidator.ThrowIfNullOrEmpty("redirectUri", redirectUri);
			ArgumentValidator.ThrowIfNullOrEmpty("graphTokenEndpoint", graphTokenEndpoint);
			ArgumentValidator.ThrowIfNull("authClient", authClient);
			FacebookAuthenticatorConfig facebookAuthenticatorConfig = new FacebookAuthenticatorConfig();
			facebookAuthenticatorConfig.AppId = appId;
			facebookAuthenticatorConfig.AppSecret = appSecret;
			facebookAuthenticatorConfig.RedirectUri = redirectUri;
			facebookAuthenticatorConfig.GraphTokenEndpoint = graphTokenEndpoint;
			facebookAuthenticatorConfig.AuthenticationClient = authClient;
			if (!TimeSpan.Zero.Equals(webRequestTimeout))
			{
				facebookAuthenticatorConfig.WebRequestTimeout = webRequestTimeout;
			}
			facebookAuthenticatorConfig.ReadTimeUtc = readTimeUtc;
			return facebookAuthenticatorConfig;
		}

		public string AuthorizationEndpoint { get; private set; }

		public string GraphTokenEndpoint { get; private set; }

		public string AppId { get; private set; }

		public string AppSecret { get; private set; }

		public string Locale { get; private set; }

		public string RedirectUri { get; private set; }

		public string Scope { get; private set; }

		public IFacebookAuthenticationWebClient AuthenticationClient { get; private set; }

		public TimeSpan WebRequestTimeout { get; private set; }

		public DateTime ReadTimeUtc { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{{ App id: {0};  Authorization endpoint: {1};  Graph token endpoint: {2};  Redirect URI: {3};  Web request timeout: {4}; Locale {5};  App secret hash: {6:X8};  Read time: {7:u} }}", new object[]
			{
				this.AppId,
				this.AuthorizationEndpoint,
				this.GraphTokenEndpoint,
				this.RedirectUri,
				this.WebRequestTimeout,
				this.Locale,
				this.GetAppSecretHashCode(),
				this.ReadTimeUtc
			});
		}

		private static string GetFacebookLocaleName(CultureInfo culture)
		{
			return culture.Name.Replace('-', '_');
		}

		private int GetAppSecretHashCode()
		{
			if (this.AppSecret == null)
			{
				return 0;
			}
			return this.AppSecret.GetHashCode();
		}

		internal static readonly string DefaultPermissions = string.Join(",", new string[]
		{
			"offline_access",
			"user_about_me",
			"friends_about_me",
			"email",
			"user_activities",
			"friends_activities",
			"user_birthday",
			"friends_birthday",
			"user_education_history",
			"friends_education_history",
			"user_hometown",
			"friends_hometown",
			"user_interests",
			"friends_interests",
			"user_website",
			"friends_website",
			"user_work_history",
			"friends_work_history",
			"user_status",
			"friends_status",
			"user_photo_video_tags",
			"friends_photo_video_tags",
			"user_photos",
			"friends_photos",
			"user_videos",
			"friends_videos",
			"friends_location",
			"friends_interests"
		});

		private static readonly TimeSpan DefaultWebRequestTimeout = TimeSpan.FromSeconds(10.0);
	}
}
