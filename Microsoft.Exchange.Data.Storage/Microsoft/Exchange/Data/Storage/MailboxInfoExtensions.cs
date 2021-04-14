using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MailboxInfoExtensions
	{
		public static Guid GetDatabaseGuid(this IMailboxInfo mailboxInfo)
		{
			ArgumentValidator.ThrowIfNull("mailboxInfo", mailboxInfo);
			if (mailboxInfo.MailboxDatabase.IsNullOrEmpty())
			{
				return Guid.Empty;
			}
			return mailboxInfo.MailboxDatabase.ObjectGuid;
		}

		public static string GetMailboxLegacyDn(this IMailboxInfo mailboxInfo, string userLegacyDn)
		{
			if (mailboxInfo.IsArchive || mailboxInfo.IsAggregated)
			{
				return userLegacyDn + "/guid=" + mailboxInfo.MailboxGuid;
			}
			return userLegacyDn;
		}
	}
}
