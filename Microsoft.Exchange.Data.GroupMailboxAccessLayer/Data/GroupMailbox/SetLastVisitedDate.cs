using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetLastVisitedDate : UpdateAssociationCommand
	{
		public SetLastVisitedDate(IExtensibleLogger logger, ExDateTime lastVisitedDate, IUserAssociationAdaptor masterAdaptor, UserMailboxLocator itemLocator) : base(logger, masterAdaptor, new IMailboxLocator[]
		{
			itemLocator
		})
		{
			this.lastVisitedDate = lastVisitedDate;
		}

		protected override bool UpdateAssociation(MailboxAssociation association)
		{
			association.LastVisitedDate = this.lastVisitedDate;
			return true;
		}

		private readonly ExDateTime lastVisitedDate;
	}
}
