using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal enum SourceType
	{
		LegacyExchangeDN,
		PublicFolder,
		Recipient,
		MailboxGuid,
		AllPublicFolders,
		AllMailboxes,
		SavedSearchId,
		AutoDetect
	}
}
