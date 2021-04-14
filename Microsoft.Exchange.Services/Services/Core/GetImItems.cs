using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetImItems
	{
		public GetImItems(IMailboxSession session, StoreId[] contactIds, StoreId[] groupIds, ExtendedPropertyUri[] extendedProperties, IXSOFactory xsoFactory)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, xsoFactory);
			this.contactIds = contactIds;
			this.groupIds = groupIds;
			this.extendedProperties = extendedProperties;
		}

		public RawImItemList Execute()
		{
			return this.unifiedContactStoreUtilities.GetImItems(this.contactIds, this.groupIds, this.extendedProperties);
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly StoreId[] contactIds;

		private readonly StoreId[] groupIds;

		private readonly ExtendedPropertyUri[] extendedProperties;
	}
}
