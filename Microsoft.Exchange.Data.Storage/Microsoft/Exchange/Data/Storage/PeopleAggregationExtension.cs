using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PeopleAggregationExtension : AggregationExtension
	{
		public PeopleAggregationExtension(MailboxSession mailboxSession)
		{
			this.contactFolders = mailboxSession.ContactFolders;
			this.clientInfoString = mailboxSession.ClientInfoString;
		}

		public override PropertyAggregationContext GetPropertyAggregationContext(IList<IStorePropertyBag> sources)
		{
			return new PersonPropertyAggregationContext(sources, this.contactFolders, this.clientInfoString);
		}

		private readonly ContactFolders contactFolders;

		private readonly string clientInfoString;
	}
}
