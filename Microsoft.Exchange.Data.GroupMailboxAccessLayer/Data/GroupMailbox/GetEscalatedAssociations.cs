using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetEscalatedAssociations : GetAssociationCommand
	{
		public GetEscalatedAssociations(IAssociationAdaptor adaptor)
		{
			this.AssociationAdaptor = adaptor;
		}

		public static int GetEscalatedAssociationsCount(IAssociationAdaptor adaptor)
		{
			GetEscalatedAssociations getEscalatedAssociations = new GetEscalatedAssociations(adaptor);
			IEnumerable<MailboxAssociation> source = getEscalatedAssociations.Execute(null);
			return source.ToArray<MailboxAssociation>().Count<MailboxAssociation>();
		}

		public override IEnumerable<MailboxAssociation> Execute(int? maxItems = null)
		{
			return this.AssociationAdaptor.GetEscalatedAssociations();
		}

		public readonly IAssociationAdaptor AssociationAdaptor;
	}
}
