using System;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class EcpUtilities
	{
		public static bool TryGetPopSubscriptionDetailsUrl(IExchangePrincipal subscriptionExchangePrincipal, PopAggregationSubscription popSubscription, SyncLogSession syncLogSession, out string popSubscriptionDetailsUrl)
		{
			popSubscriptionDetailsUrl = null;
			SyncUtilities.ThrowIfArgumentNull("subscriptionExchangePrincipal", subscriptionExchangePrincipal);
			SyncUtilities.ThrowIfArgumentNull("popSubscription", popSubscription);
			string text;
			if (!EcpUtilities.TryGetBaseEcpUrl(subscriptionExchangePrincipal, syncLogSession, out text))
			{
				return false;
			}
			string text2 = HttpUtility.UrlEncode(popSubscription.SubscriptionIdentity.ToString());
			popSubscriptionDetailsUrl = string.Format(CultureInfo.InvariantCulture, "{0}PersonalSettings/EditPopSubscription.aspx?exsvurl=1&id={1}", new object[]
			{
				text,
				text2
			});
			popSubscriptionDetailsUrl = EcpUtilities.AppendRealmForEcpUrl(subscriptionExchangePrincipal, popSubscriptionDetailsUrl, syncLogSession);
			return true;
		}

		public static bool TryGetHotmailSubscriptionDetailsUrl(IExchangePrincipal subscriptionExchangePrincipal, DeltaSyncAggregationSubscription hotmailSubscription, SyncLogSession syncLogSession, out string hotmailSubscriptionDetailsUrl)
		{
			hotmailSubscriptionDetailsUrl = null;
			SyncUtilities.ThrowIfArgumentNull("subscriptionExchangePrincipal", subscriptionExchangePrincipal);
			SyncUtilities.ThrowIfArgumentNull("hotmailSubscription", hotmailSubscription);
			string text;
			if (!EcpUtilities.TryGetBaseEcpUrl(subscriptionExchangePrincipal, syncLogSession, out text))
			{
				return false;
			}
			string text2 = HttpUtility.UrlEncode(hotmailSubscription.SubscriptionIdentity.ToString());
			hotmailSubscriptionDetailsUrl = string.Format(CultureInfo.InvariantCulture, "{0}PersonalSettings/EditHotmailSubscription.aspx?exsvurl=1&id={1}", new object[]
			{
				text,
				text2
			});
			hotmailSubscriptionDetailsUrl = EcpUtilities.AppendRealmForEcpUrl(subscriptionExchangePrincipal, hotmailSubscriptionDetailsUrl, syncLogSession);
			return true;
		}

		public static bool TryGetImapSubscriptionDetailsUrl(IExchangePrincipal subscriptionExchangePrincipal, IMAPAggregationSubscription imapSubscription, SyncLogSession syncLogSession, out string imapSubscriptionDetailsUrl)
		{
			imapSubscriptionDetailsUrl = null;
			SyncUtilities.ThrowIfArgumentNull("subscriptionExchangePrincipal", subscriptionExchangePrincipal);
			SyncUtilities.ThrowIfArgumentNull("imapSubscription", imapSubscription);
			string text;
			if (!EcpUtilities.TryGetBaseEcpUrl(subscriptionExchangePrincipal, syncLogSession, out text))
			{
				return false;
			}
			string text2 = HttpUtility.UrlEncode(imapSubscription.SubscriptionIdentity.ToString());
			imapSubscriptionDetailsUrl = string.Format(CultureInfo.InvariantCulture, "{0}PersonalSettings/EditImapSubscription.aspx?exsvurl=1&id={1}", new object[]
			{
				text,
				text2
			});
			imapSubscriptionDetailsUrl = EcpUtilities.AppendRealmForEcpUrl(subscriptionExchangePrincipal, imapSubscriptionDetailsUrl, syncLogSession);
			return true;
		}

		public static bool TryGetSendAsVerificationUrl(IExchangePrincipal subscriptionExchangePrincipal, int subscriptionTypeCode, Guid subscriptionGuid, Guid sharedSecret, SyncLogSession syncLogSession, out string sendAsVerificationUrl)
		{
			sendAsVerificationUrl = null;
			SyncUtilities.ThrowIfArgumentNull("subscriptionExchangePrincipal", subscriptionExchangePrincipal);
			string text;
			if (!EcpUtilities.TryGetBaseEcpUrl(subscriptionExchangePrincipal, syncLogSession, out text))
			{
				return false;
			}
			sendAsVerificationUrl = string.Format(CultureInfo.InvariantCulture, "{0}PersonalSettings/VerifySendAs.aspx?exsvurl=1&st={1}&su={2}&ss={3}", new object[]
			{
				text,
				HttpUtility.UrlEncode(subscriptionTypeCode.ToString()),
				HttpUtility.UrlEncode(subscriptionGuid.ToString()),
				HttpUtility.UrlEncode(sharedSecret.ToString())
			});
			sendAsVerificationUrl = EcpUtilities.AppendRealmForEcpUrl(subscriptionExchangePrincipal, sendAsVerificationUrl, syncLogSession);
			return true;
		}

		private static bool TryGetBaseEcpUrl(IExchangePrincipal subscriptionExchangePrincipal, SyncLogSession syncLogSession, out string baseEcpUrl)
		{
			baseEcpUrl = null;
			Uri uri = null;
			try
			{
				uri = FrontEndLocator.GetFrontEndEcpUrl(subscriptionExchangePrincipal);
			}
			catch (ServerNotFoundException)
			{
			}
			if (uri == null)
			{
				syncLogSession.LogError((TSLID)1UL, "Unable to retrieve an ECP base URL", new object[0]);
				return false;
			}
			baseEcpUrl = uri.ToString();
			return true;
		}

		private static string AppendRealmForEcpUrl(IExchangePrincipal subscriptionExchangePrincipal, string ecpUrl, SyncLogSession syncLogSession)
		{
			Guid externalDirectoryOrganizationId = new Guid(subscriptionExchangePrincipal.MailboxInfo.OrganizationId.ToExternalDirectoryOrganizationId());
			ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 281, "AppendRealmForEcpUrl", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Common\\EcpUtilities.cs");
			ADUser aduser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(subscriptionExchangePrincipal.ObjectId);
			if (aduser != null)
			{
				SmtpAddress smtpAddress = new SmtpAddress(aduser.UserPrincipalName);
				if (smtpAddress.IsValidAddress)
				{
					ecpUrl = ecpUrl.Replace("exsvurl=1", "exsvurl=1&realm=" + HttpUtility.UrlEncode(smtpAddress.Domain));
				}
			}
			return ecpUrl;
		}

		private const string RealmParameter = "&realm=";

		private const string ExsvurlParameter = "exsvurl=1";

		private const string PopSubscriptionDetailsUrl = "{0}PersonalSettings/EditPopSubscription.aspx?exsvurl=1&id={1}";

		private const string HotmailSubscriptionDetailsUrl = "{0}PersonalSettings/EditHotmailSubscription.aspx?exsvurl=1&id={1}";

		private const string ImapSubscriptionDetailsUrl = "{0}PersonalSettings/EditImapSubscription.aspx?exsvurl=1&id={1}";

		private const string SendAsVerificationUrl = "{0}PersonalSettings/VerifySendAs.aspx?exsvurl=1&st={1}&su={2}&ss={3}";
	}
}
