using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxInfo
	{
		string DisplayName { get; }

		SmtpAddress PrimarySmtpAddress { get; }

		ProxyAddress ExternalEmailAddress { get; }

		IEnumerable<ProxyAddress> EmailAddresses { get; }

		OrganizationId OrganizationId { get; }

		Guid MailboxGuid { get; }

		ADObjectId MailboxDatabase { get; }

		DateTime? WhenMailboxCreated { get; }

		string ArchiveName { get; }

		bool IsArchive { get; }

		bool IsAggregated { get; }

		ArchiveStatusFlags ArchiveStatus { get; }

		ArchiveState ArchiveState { get; }

		SmtpAddress? RemoteIdentity { get; }

		bool IsRemote { get; }

		IMailboxLocation Location { get; }

		IMailboxConfiguration Configuration { get; }

		MailboxLocationType MailboxType { get; }
	}
}
