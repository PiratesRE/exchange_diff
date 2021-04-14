using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetMembershipChangedAfterAssociations : GetAssociationCommand
	{
		public GetMembershipChangedAfterAssociations(IAssociationAdaptor adaptor, ExDateTime date)
		{
			this.AssociationAdaptor = adaptor;
			this.changeDate = date;
		}

		public override IEnumerable<MailboxAssociation> Execute(int? maxItems = null)
		{
			return this.AssociationAdaptor.GetAssociationsWithMembershipChangedAfter(this.changeDate);
		}

		public readonly IAssociationAdaptor AssociationAdaptor;

		private readonly ExDateTime changeDate;
	}
}
