using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.LinkedIn;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NewLinkedInSubscription : INewConnectSubscription
	{
		internal NewLinkedInSubscription()
		{
		}

		public string SubscriptionName
		{
			get
			{
				return WellKnownNetworkNames.LinkedIn;
			}
		}

		public string SubscriptionDisplayName
		{
			get
			{
				return WellKnownNetworkNames.LinkedIn;
			}
		}

		public IConfigurable PrepareSubscription(MailboxSession mailbox, ConnectSubscriptionProxy proxy)
		{
			ArgumentValidator.ThrowIfNull("mailbox", mailbox);
			ArgumentValidator.ThrowIfNull("proxy", proxy);
			this.InitializeConfiguration();
			LinkedInTokenInformation linkedInTokenInformation = this.ExchangeRequestTokenForAccessTokenAndSecret(proxy.RequestToken, proxy.RequestSecret, proxy.OAuthVerifier);
			proxy.Subscription.SubscriptionType = AggregationSubscriptionType.LinkedIn;
			proxy.Subscription.SubscriptionProtocolName = ConnectSubscription.LinkedInProtocolName;
			proxy.Subscription.SubscriptionProtocolVersion = ConnectSubscription.LinkedInProtocolVersion;
			proxy.Subscription.SubscriptionEvents = SubscriptionEvents.WorkItemCompleted;
			proxy.SendAsCheckNeeded = false;
			proxy.ProviderGuid = ConnectSubscription.LinkedInProviderGuid;
			proxy.MessageClass = "IPM.Aggregation.LinkedIn";
			proxy.AssignAccessToken(linkedInTokenInformation.Token);
			proxy.AssignAccessTokenSecret(linkedInTokenInformation.Secret);
			proxy.AppId = this.authConfig.AppId;
			proxy.UserId = this.GetUserId(proxy.RequestToken, linkedInTokenInformation.Token, linkedInTokenInformation.Secret);
			OscFolderCreateResult oscFolderCreateResult = new OscFolderCreator(mailbox).Create("LinkedIn", proxy.UserId);
			if (!oscFolderCreateResult.Created)
			{
				new OscFolderMigration(mailbox, OscContactSourcesForContactParser.Instance).Migrate(oscFolderCreateResult.FolderId);
				proxy.InitialSyncInRecoveryMode = true;
			}
			return proxy;
		}

		public void InitializeFolderAndNotifyApps(MailboxSession mailbox, ConnectSubscriptionProxy subscription)
		{
			if (mailbox == null)
			{
				throw new ArgumentNullException("mailbox");
			}
			if (subscription == null)
			{
				throw new ArgumentNullException("subscription");
			}
			if (NewLinkedInSubscription.IsTestHook(subscription.RequestToken))
			{
				return;
			}
			new OscSyncLockCreator(mailbox).Create("LinkedIn", subscription.UserId);
			new PeopleConnectNotifier(mailbox).NotifyConnected(WellKnownNetworkNames.LinkedIn);
		}

		private void InitializeConfiguration()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadLinkedIn();
			this.authConfig = LinkedInConfig.CreateForAppAuth(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.RequestTokenEndpoint, peopleConnectApplicationConfig.AccessTokenEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri, peopleConnectApplicationConfig.ReadTimeUtc);
			this.appConfig = new LinkedInAppConfig(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.ProfileEndpoint, peopleConnectApplicationConfig.ConnectionsEndpoint, peopleConnectApplicationConfig.RemoveAppEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri);
		}

		private LinkedInTokenInformation ExchangeRequestTokenForAccessTokenAndSecret(string requestToken, string requestSecret, string oauthVerifier)
		{
			if (NewLinkedInSubscription.IsTestHook(requestToken))
			{
				return new LinkedInTokenInformation
				{
					Token = "***TEST_ACCESS_TOKEN***",
					Secret = "***TEST_ACCESS_TOKEN_SECRET***"
				};
			}
			return this.CreateAuthenticator().GetAccessToken(requestToken, requestSecret, oauthVerifier);
		}

		private string GetUserId(string requestToken, string accessToken, string accessTokenSecret)
		{
			if (NewLinkedInSubscription.IsTestHook(requestToken))
			{
				return "***TEST_USER_ID***";
			}
			return new LinkedInWebClient(this.appConfig, NewLinkedInSubscription.Tracer).GetProfile(accessToken, accessTokenSecret, "email-address").EmailAddress;
		}

		private static bool IsTestHook(string requestToken)
		{
			return "***TEST_REQUEST_TOKEN***".Equals(requestToken, StringComparison.Ordinal);
		}

		private LinkedInAuthenticator CreateAuthenticator()
		{
			return new LinkedInAuthenticator(this.authConfig, new LinkedInWebClient(this.appConfig, NewLinkedInSubscription.Tracer), NewLinkedInSubscription.Tracer);
		}

		private const string TestRequestToken = "***TEST_REQUEST_TOKEN***";

		private const string TestAccessToken = "***TEST_ACCESS_TOKEN***";

		private const string TestAccessTokenSecret = "***TEST_ACCESS_TOKEN_SECRET***";

		private const string TestUserId = "***TEST_USER_ID***";

		private static readonly Trace Tracer = ExTraceGlobals.LinkedInTracer;

		private LinkedInConfig authConfig;

		private LinkedInAppConfig appConfig;
	}
}
