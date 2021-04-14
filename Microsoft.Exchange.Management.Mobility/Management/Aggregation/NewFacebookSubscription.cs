using System;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Facebook;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NewFacebookSubscription : INewConnectSubscription
	{
		internal NewFacebookSubscription()
		{
		}

		public string SubscriptionName
		{
			get
			{
				return WellKnownNetworkNames.Facebook;
			}
		}

		public string SubscriptionDisplayName
		{
			get
			{
				return WellKnownNetworkNames.Facebook;
			}
		}

		public IConfigurable PrepareSubscription(MailboxSession mailbox, ConnectSubscriptionProxy proxy)
		{
			ArgumentValidator.ThrowIfNull("proxy", proxy);
			NewFacebookSubscription.Tracer.TraceFunction((long)this.GetHashCode(), "Entering NewFacebookSubscription.PrepareSubscription. Mailbox {0}.", new object[]
			{
				mailbox.Identity
			});
			this.InitializeConfiguration(proxy.AppAuthorizationCode, proxy.RedirectUri);
			string text;
			string userId;
			if (NewFacebookSubscription.IsTestHook(proxy.AppAuthorizationCode))
			{
				text = "***TEST_ACCESS_TOKEN***";
				userId = "***TEST_USER_ID***";
			}
			else
			{
				text = new FacebookAuthenticator(this.config).ExchangeAppAuthorizationCodeForAccessToken(proxy.AppAuthorizationCode);
				userId = NewFacebookSubscription.GetUserId(text);
				if (!CachedPeopleConnectApplicationConfig.Instance.ReadFacebook().SkipContactUpload)
				{
					NewFacebookSubscription.UploadContacts(mailbox, text);
				}
				else
				{
					NewFacebookSubscription.Tracer.TraceWarning<string>((long)this.GetHashCode(), "Skipping contact upload for new subscription in Mailbox {0}.", mailbox.Identity.ToString());
				}
			}
			proxy.Subscription.SubscriptionType = AggregationSubscriptionType.Facebook;
			proxy.Subscription.SubscriptionProtocolName = ConnectSubscription.FacebookProtocolName;
			proxy.Subscription.SubscriptionProtocolVersion = ConnectSubscription.FacebookProtocolVersion;
			proxy.Subscription.SubscriptionEvents = SubscriptionEvents.WorkItemCompleted;
			proxy.SendAsCheckNeeded = false;
			proxy.ProviderGuid = ConnectSubscription.FacebookProviderGuid;
			proxy.MessageClass = "IPM.Aggregation.Facebook";
			proxy.AssignAccessToken(text);
			proxy.AppId = this.config.AppId;
			proxy.UserId = userId;
			NewFacebookSubscription.Tracer.TraceFunction((long)this.GetHashCode(), "Leaving NewFacebookSubscription.PrepareSubscription. Mailbox {0}.", new object[]
			{
				mailbox.Identity
			});
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
			if (NewFacebookSubscription.IsTestHook(subscription.AppAuthorizationCode))
			{
				return;
			}
			new OscFolderCreator(mailbox).Create("Facebook", subscription.UserId);
			new OscSyncLockCreator(mailbox).Create("Facebook", subscription.UserId);
			new PeopleConnectNotifier(mailbox).NotifyConnected(WellKnownNetworkNames.Facebook);
		}

		private static string GetUserId(string accessToken)
		{
			if (string.IsNullOrEmpty(accessToken))
			{
				throw new FacebookNewSubscriptionException(Strings.FacebookEmptyAccessToken);
			}
			string result = null;
			try
			{
				string graphApiEndpoint = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook().GraphApiEndpoint;
				using (FacebookClient facebookClient = new FacebookClient(new Uri(graphApiEndpoint)))
				{
					FacebookUser profile = facebookClient.GetProfile(accessToken, "id");
					if (profile != null)
					{
						result = profile.Id;
					}
				}
			}
			catch (TimeoutException innerException)
			{
				throw new FacebookNewSubscriptionException(Strings.FacebookTimeoutError, innerException);
			}
			catch (ProtocolException innerException2)
			{
				throw new FacebookNewSubscriptionException(Strings.FacebookAuthorizationError, innerException2);
			}
			catch (CommunicationException innerException3)
			{
				throw new FacebookNewSubscriptionException(Strings.FacebookCommunicationError, innerException3);
			}
			return result;
		}

		private static void UploadContacts(MailboxSession mailboxSession, string accessToken)
		{
			NewFacebookSubscription.Tracer.TraceFunction(0L, "Entering NewFacebookSubscription.UploadContacts. Mailbox {0}.", new object[]
			{
				mailboxSession.Identity
			});
			try
			{
				IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook();
				string graphApiEndpoint = peopleConnectApplicationConfig.GraphApiEndpoint;
				using (FacebookClient facebookClient = new FacebookClient(new Uri(graphApiEndpoint)))
				{
					ContactsUploaderPerformanceTracker contactsUploaderPerformanceTracker = new ContactsUploaderPerformanceTracker();
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						currentActivityScope.UserState = contactsUploaderPerformanceTracker;
					}
					else
					{
						NewFacebookSubscription.Tracer.TraceFunction(0L, "Can't add metadata for Contacts upload as there is no current activity scope.");
					}
					FacebookContactsUploader facebookContactsUploader = new FacebookContactsUploader(contactsUploaderPerformanceTracker, facebookClient, peopleConnectApplicationConfig, (PropertyDefinition[] propertiesToLoad) => new RecursiveContactsEnumerator(mailboxSession, new XSOFactory(), DefaultFolderType.Contacts, propertiesToLoad));
					facebookContactsUploader.UploadContacts(accessToken);
				}
			}
			finally
			{
				NewFacebookSubscription.Tracer.TraceFunction(0L, "Leaving NewFacebookSubscription.UploadContacts. Mailbox {0}.", new object[]
				{
					mailboxSession.Identity
				});
			}
		}

		private void InitializeConfiguration(string appAuthorizationCode, string redirectUri)
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook();
			this.config = FacebookAuthenticatorConfig.CreateForAppAuthentication(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, redirectUri, peopleConnectApplicationConfig.GraphTokenEndpoint, new FacebookAuthenticationWebClient(), peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.ReadTimeUtc);
		}

		private static bool IsTestHook(string appAuthorizationCode)
		{
			return "***TEST_APPCODE***".Equals(appAuthorizationCode, StringComparison.Ordinal);
		}

		private const string TestAppAuthorizationCode = "***TEST_APPCODE***";

		private const string TestAccessToken = "***TEST_ACCESS_TOKEN***";

		private const string TestUserId = "***TEST_USER_ID***";

		internal static readonly Trace Tracer = ExTraceGlobals.FacebookTracer;

		private FacebookAuthenticatorConfig config;
	}
}
