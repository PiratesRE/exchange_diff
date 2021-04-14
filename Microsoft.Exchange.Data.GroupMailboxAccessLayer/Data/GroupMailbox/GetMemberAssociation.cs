using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetMemberAssociation
	{
		public GetMemberAssociation(IAssociationAdaptor adaptor, IMailboxLocator user)
		{
			this.user = user;
			this.associationAdaptor = adaptor;
		}

		public MailboxAssociation Execute()
		{
			return this.associationAdaptor.GetAssociation(this.user);
		}

		private readonly IAssociationAdaptor associationAdaptor;

		private readonly IMailboxLocator user;
	}
}
