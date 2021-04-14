using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.LinkedIn;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SetLinkedInSubscription : ISetConnectSubscription
	{
		public void StampChangesOn(ConnectSubscriptionProxy proxy)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}
			this.InitializeConfiguration();
			LinkedInTokenInformation linkedInTokenInformation = this.ExchangeRequestTokenForAccessTokenAndSecret(proxy.RequestToken, proxy.RequestSecret, proxy.OAuthVerifier);
			this.RejectIfDifferentAccount(proxy, proxy.RequestToken, linkedInTokenInformation.Token, linkedInTokenInformation.Secret);
			proxy.AssignAccessToken(linkedInTokenInformation.Token);
			proxy.AssignAccessTokenSecret(linkedInTokenInformation.Secret);
			proxy.AppId = this.authConfig.AppId;
		}

		public void NotifyApps(MailboxSession mailbox)
		{
			if (mailbox == null)
			{
				throw new ArgumentNullException("mailbox");
			}
			new PeopleConnectNotifier(mailbox).NotifyConnected(WellKnownNetworkNames.LinkedIn);
		}

		private void InitializeConfiguration()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadLinkedIn();
			this.authConfig = LinkedInConfig.CreateForAppAuth(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.RequestTokenEndpoint, peopleConnectApplicationConfig.AccessTokenEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri, peopleConnectApplicationConfig.ReadTimeUtc);
			this.appConfig = new LinkedInAppConfig(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.ProfileEndpoint, peopleConnectApplicationConfig.ConnectionsEndpoint, peopleConnectApplicationConfig.RemoveAppEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri);
		}

		private void RejectIfDifferentAccount(ConnectSubscriptionProxy proxy, string requestToken, string newAccessToken, string newAccessTokenSecret)
		{
			if (SetLinkedInSubscription.IsTestHook(requestToken))
			{
				return;
			}
			string emailAddress = new LinkedInWebClient(this.appConfig, SetLinkedInSubscription.Tracer).GetProfile(newAccessToken, newAccessTokenSecret, "email-address").EmailAddress;
			if (!string.Equals(proxy.UserId, emailAddress, StringComparison.Ordinal))
			{
				throw new CannotSwitchLinkedInAccountException();
			}
		}

		private LinkedInTokenInformation ExchangeRequestTokenForAccessTokenAndSecret(string requestToken, string requestSecret, string oauthVerifier)
		{
			if (SetLinkedInSubscription.IsTestHook(requestToken))
			{
				return new LinkedInTokenInformation
				{
					Token = "***TEST_SET_ACCESS_TOKEN***",
					Secret = "***TEST_SET_ACCESS_TOKEN_SECRET***"
				};
			}
			return this.CreateAuthenticator().GetAccessToken(requestToken, requestSecret, oauthVerifier);
		}

		private static bool IsTestHook(string requestToken)
		{
			return "***TEST_SET_REQUEST_TOKEN***".Equals(requestToken, StringComparison.Ordinal);
		}

		private LinkedInAuthenticator CreateAuthenticator()
		{
			return new LinkedInAuthenticator(this.authConfig, new LinkedInWebClient(this.appConfig, SetLinkedInSubscription.Tracer), SetLinkedInSubscription.Tracer);
		}

		private const string TestRequestToken = "***TEST_SET_REQUEST_TOKEN***";

		private const string TestAccessToken = "***TEST_SET_ACCESS_TOKEN***";

		private const string TestAccessTokenSecret = "***TEST_SET_ACCESS_TOKEN_SECRET***";

		private const string TestUserId = "***TEST_SET_USER_ID***";

		private static readonly Trace Tracer = ExTraceGlobals.LinkedInTracer;

		private LinkedInConfig authConfig;

		private LinkedInAppConfig appConfig;
	}
}
