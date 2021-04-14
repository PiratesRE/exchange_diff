using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetMembershipAssociations : GetAssociationCommand
	{
		public GetMembershipAssociations(IAssociationAdaptor adaptor)
		{
			this.AssociationAdaptor = adaptor;
		}

		public override IEnumerable<MailboxAssociation> Execute(int? maxItems = null)
		{
			return this.AssociationAdaptor.GetMembershipAssociations(maxItems);
		}

		public readonly IAssociationAdaptor AssociationAdaptor;
	}
}
