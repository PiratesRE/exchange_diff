using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class GetAssociationCommand
	{
		public abstract IEnumerable<MailboxAssociation> Execute(int? maxItems = null);
	}
}
