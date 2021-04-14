using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Autodiscover
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutodiscoverCommonUserSettings
	{
		internal string AccountDisplayName { get; private set; }

		internal string AccountLegacyDn { get; private set; }

		internal SmtpAddress PrimarySmtpAddress { get; private set; }

		internal string RpcServer { get; private set; }

		internal Guid MailboxGuid { get; private set; }

		private AutodiscoverCommonUserSettings(string accountDisplayName, string accountLegacyDn, SmtpAddress primarySmtpAddress, string rpcServer, Guid mailboxGuid)
		{
			this.AccountDisplayName = accountDisplayName;
			this.AccountLegacyDn = accountLegacyDn;
			this.PrimarySmtpAddress = primarySmtpAddress;
			this.RpcServer = rpcServer;
			this.MailboxGuid = mailboxGuid;
		}

		internal static AutodiscoverCommonUserSettings GetSettingsFromRecipient(ADUser user, string emailAddress)
		{
			bool flag = AutodiscoverCommonUserSettings.IsArchiveMailUser(user) || AutodiscoverCommonUserSettings.IsEmailAddressTargetingArchive(user, emailAddress);
			string domain = user.PrimarySmtpAddress.Domain;
			Guid mailboxGuid = flag ? user.ArchiveGuid : user.ExchangeGuid;
			return new AutodiscoverCommonUserSettings(flag ? AutodiscoverCommonUserSettings.GetArchiveDisplayName(user) : user.DisplayName, flag ? user.GetAlternateMailboxLegDN(user.ArchiveGuid) : user.LegacyExchangeDN, user.PrimarySmtpAddress, ExchangeRpcClientAccess.CreatePersonalizedServer(mailboxGuid, domain), mailboxGuid);
		}

		internal static bool IsArchiveMailUser(ADRecipient recipient)
		{
			return recipient.RecipientType == RecipientType.MailUser && AutodiscoverCommonUserSettings.HasLocalArchive(recipient);
		}

		internal static string GetArchiveDisplayName(ADUser user)
		{
			if (user.ArchiveName == null || user.ArchiveName.Count != 1)
			{
				return "Online Archive";
			}
			return user.ArchiveName[0];
		}

		internal static bool HasLocalArchive(ADRecipient recipient)
		{
			ADUser aduser = recipient as ADUser;
			return aduser != null && !aduser.ArchiveGuid.Equals(Guid.Empty) && aduser.ArchiveDatabase != null && !aduser.ArchiveDatabase.ObjectGuid.Equals(Guid.Empty);
		}

		internal static bool IsEmailAddressTargetingArchive(ADUser adUser, string emailAddress)
		{
			if (adUser == null || string.IsNullOrEmpty(emailAddress))
			{
				return false;
			}
			bool result = false;
			Guid empty = Guid.Empty;
			if (AutodiscoverCommonUserSettings.TryGetExchangeGuidFromEmailAddress(emailAddress, out empty))
			{
				result = empty.Equals(adUser.ArchiveGuid);
			}
			return result;
		}

		internal static bool TryGetExchangeGuidFromEmailAddress(string emailAddress, out Guid exchangeGuid)
		{
			exchangeGuid = Guid.Empty;
			if (string.IsNullOrEmpty(emailAddress) || !SmtpAddress.IsValidSmtpAddress(emailAddress))
			{
				return false;
			}
			if (SmtpProxyAddress.TryDeencapsulateExchangeGuid(emailAddress, out exchangeGuid))
			{
				return true;
			}
			SmtpAddress smtpAddress = new SmtpAddress(emailAddress);
			return !string.IsNullOrEmpty(smtpAddress.Local) && Guid.TryParse(smtpAddress.Local, out exchangeGuid);
		}
	}
}
