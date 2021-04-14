using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxLocator
	{
		string LegacyDn { get; }

		string ExternalId { get; }

		Guid MailboxGuid { get; }

		string LocatorType { get; }

		string IdentityHash { get; }

		ADUser FindAdUser();

		string[] FindAlternateLegacyDNs();

		bool IsValidReplicationTarget();
	}
}
