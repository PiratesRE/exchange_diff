using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemoveDistributionGroupFromImList
	{
		public RemoveDistributionGroupFromImList(IMailboxSession session, StoreId groupId, IXSOFactory xsoFactory)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (groupId == null)
			{
				throw new ArgumentNullException("groupId");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, xsoFactory);
			this.groupId = groupId;
		}

		public void Execute()
		{
			this.unifiedContactStoreUtilities.RemoveDistributionGroupFromImList(this.groupId);
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly StoreId groupId;
	}
}
