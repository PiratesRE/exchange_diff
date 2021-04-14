using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetImGroup
	{
		internal SetImGroup(IMailboxSession session, StoreId groupId, string newDisplayName, IXSOFactory xsoFactory)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (groupId == null)
			{
				throw new ArgumentNullException("groupId");
			}
			if (newDisplayName == null)
			{
				throw new ArgumentNullException("newDisplayName");
			}
			if (newDisplayName.Length == 0)
			{
				throw new ArgumentException("newDisplayName that was passed in was empty.", "newDisplayName");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, xsoFactory);
			this.groupId = groupId;
			this.newDisplayName = newDisplayName;
		}

		internal void Execute()
		{
			this.unifiedContactStoreUtilities.ModifyGroup(this.groupId, this.newDisplayName);
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly StoreId groupId;

		private readonly string newDisplayName;
	}
}
