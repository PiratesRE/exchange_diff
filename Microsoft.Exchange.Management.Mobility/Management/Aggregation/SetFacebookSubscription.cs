using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Facebook;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SetFacebookSubscription : ISetConnectSubscription
	{
		public void StampChangesOn(ConnectSubscriptionProxy proxy)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}
			this.InitializeConfiguration(proxy.RedirectUri);
			string text;
			string userId;
			if (SetFacebookSubscription.IsTestHook(proxy.AppAuthorizationCode))
			{
				text = "***TEST_SET_ACCESS_TOKEN***";
				userId = "***TEST_SET_USER_ID***";
			}
			else
			{
				text = new FacebookAuthenticator(this.config).ExchangeAppAuthorizationCodeForAccessToken(proxy.AppAuthorizationCode);
				userId = SetFacebookSubscription.GetUserId(text);
			}
			proxy.AssignAccessToken(text);
			proxy.AppId = this.config.AppId;
			proxy.UserId = userId;
		}

		private static string GetUserId(string accessToken)
		{
			if (string.IsNullOrEmpty(accessToken))
			{
				throw new FacebookUpdateSubscriptionException(Strings.FacebookEmptyAccessToken);
			}
			string result = null;
			try
			{
				string graphApiEndpoint = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook().GraphApiEndpoint;
				FacebookUser profile = new FacebookClient(new Uri(graphApiEndpoint)).GetProfile(accessToken, "id");
				if (profile != null)
				{
					result = profile.Id;
				}
			}
			catch (TimeoutException innerException)
			{
				throw new FacebookUpdateSubscriptionException(Strings.FacebookTimeoutError, innerException);
			}
			catch (ProtocolException innerException2)
			{
				throw new FacebookUpdateSubscriptionException(Strings.FacebookAuthorizationError, innerException2);
			}
			catch (CommunicationException innerException3)
			{
				throw new FacebookUpdateSubscriptionException(Strings.FacebookCommunicationError, innerException3);
			}
			return result;
		}

		public void NotifyApps(MailboxSession mailbox)
		{
			if (mailbox == null)
			{
				throw new ArgumentNullException("mailbox");
			}
			new PeopleConnectNotifier(mailbox).NotifyConnected(WellKnownNetworkNames.Facebook);
		}

		private void InitializeConfiguration(string redirectUri)
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook();
			this.config = FacebookAuthenticatorConfig.CreateForAppAuthentication(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, redirectUri, peopleConnectApplicationConfig.GraphTokenEndpoint, new FacebookAuthenticationWebClient(), peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.ReadTimeUtc);
		}

		private static bool IsTestHook(string appAuthorizationCode)
		{
			return "***TEST_SET_APPCODE***".Equals(appAuthorizationCode, StringComparison.Ordinal);
		}

		private const string TestAppAuthorizationCode = "***TEST_SET_APPCODE***";

		private const string TestAccessToken = "***TEST_SET_ACCESS_TOKEN***";

		private const string TestUserId = "***TEST_SET_USER_ID***";

		private FacebookAuthenticatorConfig config;
	}
}
