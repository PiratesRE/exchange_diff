using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAssociationAdaptor
	{
		event Action<IMailboxLocator> OnAfterJoin;

		IMailboxLocator MasterLocator { get; }

		IAssociationStore AssociationStore { get; }

		MailboxAssociation GetAssociation(IMailboxLocator locator);

		IEnumerable<MailboxAssociation> GetAllAssociations();

		IEnumerable<MailboxAssociation> GetMembershipAssociations(int? maxItems);

		IEnumerable<MailboxAssociation> GetEscalatedAssociations();

		IEnumerable<MailboxAssociation> GetPinAssociations();

		IEnumerable<MailboxAssociation> GetAssociationsWithMembershipChangedAfter(ExDateTime date);

		MailboxLocator GetSlaveMailboxLocator(MailboxAssociation association);

		void DeleteAssociation(MailboxAssociation association);

		void SaveAssociation(MailboxAssociation association, bool markForReplication);

		void SaveSyncState(MailboxAssociation association);

		void ReplicateAssociation(MailboxAssociation association);
	}
}
