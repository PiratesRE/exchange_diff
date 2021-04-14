using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	internal class MailboxUtilityQueryResult
	{
		internal MailboxUtilityQueryResult(string displayName, StoreObjectId storeId, StoreObjectId parentStoreId, ExDateTime lastModifiedTime)
		{
			this.displayName = displayName;
			this.storeId = storeId;
			this.parentStoreId = parentStoreId;
			this.lastModifiedTime = lastModifiedTime;
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		internal StoreObjectId StoreId
		{
			get
			{
				return this.storeId;
			}
			set
			{
				this.storeId = value;
			}
		}

		internal StoreObjectId ParentStoreId
		{
			get
			{
				return this.parentStoreId;
			}
			set
			{
				this.parentStoreId = value;
			}
		}

		internal ExDateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
			set
			{
				this.lastModifiedTime = value;
			}
		}

		private string displayName;

		private StoreObjectId storeId;

		private StoreObjectId parentStoreId;

		private ExDateTime lastModifiedTime;
	}
}
