using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.SendAsVerification;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	internal static class AggregationTaskUtils
	{
		internal static void ValidateEmailAddress(IConfigDataProvider dataProvider, PimSubscriptionProxy subscriptionProxy, Task.TaskErrorLoggingDelegate taskErrorLoggingDelegate)
		{
			if (subscriptionProxy.AggregationType != AggregationType.Migration)
			{
				ADUser aduser = ((AggregationSubscriptionDataProvider)dataProvider).ADUser;
				string text = subscriptionProxy.EmailAddress.ToString();
				if (AggregationTaskUtils.IsUserProxyAddress(aduser, text))
				{
					taskErrorLoggingDelegate(new LocalizedException(Strings.SubscriptionInvalidEmailAddress(text)), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		internal static IRecipientSession VerifyIsWithinWriteScopes(IRecipientSession recipientSession, ADUser adUser, Task.TaskErrorLoggingDelegate taskErrorLoggingDelegate)
		{
			SyncUtilities.ThrowIfArgumentNull("recipientSession", recipientSession);
			SyncUtilities.ThrowIfArgumentNull("adUser", adUser);
			SyncUtilities.ThrowIfArgumentNull("taskErrorLoggingDelegate", taskErrorLoggingDelegate);
			IRecipientSession recipientSession2 = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(recipientSession, adUser.OrganizationId, true);
			ADScopeException ex;
			if (!recipientSession2.TryVerifyIsWithinScopes(adUser, true, out ex))
			{
				taskErrorLoggingDelegate(new InvalidOperationException(Strings.ErrorCannotChangeMailboxOutOfWriteScope(adUser.Identity.ToString(), (ex == null) ? string.Empty : ex.Message), ex), ErrorCategory.InvalidOperation, adUser.Identity);
			}
			return recipientSession2;
		}

		internal static void ValidateUserName(string userName, Task.TaskErrorLoggingDelegate taskErrorLoggingDelegate)
		{
			if (string.IsNullOrEmpty(userName))
			{
				taskErrorLoggingDelegate(new InvalidOperationException(Strings.IncomingUserNameEmpty), ErrorCategory.InvalidData, null);
			}
		}

		internal static void ValidateUnicodeInfoOnUserNameAndPassword(string userName, SecureString password, Task.TaskErrorLoggingDelegate taskErrorLoggingDelegate)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("userName", userName);
			SyncUtilities.ThrowIfArgumentNull("password", password);
			SyncUtilities.ThrowIfArgumentNull("taskErrorLoggingDelegate", taskErrorLoggingDelegate);
			if (SyncUtilities.HasUnicodeCharacters(userName) || (password.Length > 0 && SyncUtilities.HasUnicodeCharacters(password)))
			{
				taskErrorLoggingDelegate(new InvalidOperationException(Strings.InvalidUnicodeCharacterUsage), ErrorCategory.InvalidArgument, null);
			}
		}

		internal static void ValidateIncomingServerLength(string incomingServer, Task.TaskErrorLoggingDelegate taskErrorLoggingDelegate)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("incomingServer", incomingServer);
			SyncUtilities.ThrowIfArgumentNull("taskErrorLoggingDelegate", taskErrorLoggingDelegate);
			if (incomingServer.Length > SyncUtilities.MaximumFqdnLength)
			{
				taskErrorLoggingDelegate(new IncomingServerTooLongException(), ErrorCategory.InvalidArgument, null);
			}
		}

		internal static void ProcessSendAsSpecificParameters(PimSubscriptionProxy subscriptionProxy, string validateSecret, bool resendVerification, AggregationSubscriptionDataProvider dataProvider, Task.TaskErrorLoggingDelegate taskErrorLoggingDelegate)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionProxy", subscriptionProxy);
			SyncUtilities.ThrowIfArgumentNull("dataProvider", dataProvider);
			SyncUtilities.ThrowIfArgumentNull("taskErrorLoggingDelegate", taskErrorLoggingDelegate);
			SendAsManager sendAsManager = new SendAsManager();
			if (validateSecret != null)
			{
				if (sendAsManager.ValidateSharedSecret(subscriptionProxy.Subscription, validateSecret))
				{
					AggregationTaskUtils.EnableAlwaysShowFrom(dataProvider.SubscriptionExchangePrincipal);
				}
				else
				{
					taskErrorLoggingDelegate(new ValidateSecretFailureException(), (ErrorCategory)1003, subscriptionProxy);
				}
			}
			if (resendVerification)
			{
				IEmailSender emailSender = subscriptionProxy.Subscription.CreateEmailSenderFor(dataProvider.ADUser, dataProvider.SubscriptionExchangePrincipal);
				sendAsManager.ResendVerificationEmail(subscriptionProxy.Subscription, emailSender);
			}
		}

		internal static void EnableAlwaysShowFrom(ExchangePrincipal subscriptionExchangePrincipal)
		{
			using (XsoDictionaryDataProvider xsoDictionaryDataProvider = new XsoDictionaryDataProvider(subscriptionExchangePrincipal, "EnableAlwaysShowFromForSendAsSubscription"))
			{
				MailboxMessageConfiguration mailboxMessageConfiguration = (MailboxMessageConfiguration)xsoDictionaryDataProvider.Read<MailboxMessageConfiguration>(null);
				mailboxMessageConfiguration.AlwaysShowFrom = true;
				xsoDictionaryDataProvider.Save(mailboxMessageConfiguration);
			}
		}

		private static bool IsUserProxyAddress(ADUser user, string email)
		{
			ProxyAddress proxyAddress = ProxyAddress.Parse(email);
			foreach (ProxyAddress other in user.EmailAddresses)
			{
				if (proxyAddress.Equals(other))
				{
					return true;
				}
			}
			return false;
		}

		private const string EnableAlwaysShowFromAction = "EnableAlwaysShowFromForSendAsSubscription";

		internal static readonly Trace Tracer = ExTraceGlobals.SubscriptionTaskTracer;
	}
}
