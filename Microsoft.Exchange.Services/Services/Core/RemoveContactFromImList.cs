using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemoveContactFromImList
	{
		public RemoveContactFromImList(IMailboxSession session, StoreId contactId, IXSOFactory xsoFactory)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (contactId == null)
			{
				throw new ArgumentNullException("contactId");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, xsoFactory);
			this.contactId = contactId;
		}

		internal void Execute()
		{
			this.unifiedContactStoreUtilities.RemoveContactFromImList(this.contactId);
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly StoreId contactId;
	}
}
