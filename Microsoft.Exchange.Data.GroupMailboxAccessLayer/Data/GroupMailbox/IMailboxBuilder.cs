using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxBuilder<T> where T : ILocatableMailbox
	{
		T Mailbox { get; }

		IMailboxBuilder<T> BuildFromAssociation(MailboxAssociation rawEntry);

		IMailboxBuilder<T> BuildFromDirectory(ADRawEntry rawEntry);
	}
}
