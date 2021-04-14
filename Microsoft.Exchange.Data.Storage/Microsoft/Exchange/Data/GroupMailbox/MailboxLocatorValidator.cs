using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MailboxLocatorValidator
	{
		public static bool IsValidUserLocator(ADUser user)
		{
			return MailboxLocatorValidator.IsValidUserLocator(user.LegacyExchangeDN, user.ExchangeVersion, user.RecipientTypeDetails);
		}

		public static bool IsValidUserLocator(string legacyExchangeDn, ExchangeObjectVersion objectVersion, RecipientTypeDetails recipientTypeDetails)
		{
			return MailboxLocatorValidator.IsValidMailboxLocator(legacyExchangeDn, objectVersion) && (recipientTypeDetails == RecipientTypeDetails.UserMailbox || recipientTypeDetails == RecipientTypeDetails.MailUser);
		}

		public static bool IsValidGroupLocator(ADUser user)
		{
			return MailboxLocatorValidator.IsValidGroupLocator(user.LegacyExchangeDN, user.ExchangeVersion, user.RecipientTypeDetails);
		}

		public static bool IsValidGroupLocator(string legacyExchangeDn, ExchangeObjectVersion objectVersion, RecipientTypeDetails recipientTypeDetails)
		{
			return MailboxLocatorValidator.IsValidMailboxLocator(legacyExchangeDn, objectVersion) && recipientTypeDetails == RecipientTypeDetails.GroupMailbox;
		}

		private static bool IsValidMailboxLocator(string legacyExchangeDn, ExchangeObjectVersion objectVersion)
		{
			return !string.IsNullOrWhiteSpace(legacyExchangeDn) && objectVersion != null;
		}
	}
}
