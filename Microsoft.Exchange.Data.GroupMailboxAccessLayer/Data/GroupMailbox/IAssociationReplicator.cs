using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAssociationReplicator
	{
		bool ReplicateAssociation(IAssociationAdaptor masterAdaptor, params MailboxAssociation[] associations);
	}
}
