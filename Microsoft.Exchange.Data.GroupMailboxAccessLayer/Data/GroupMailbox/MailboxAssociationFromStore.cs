using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MailboxAssociationFromStore : MailboxAssociation
	{
		public VersionedId ItemId { get; set; }
	}
}
