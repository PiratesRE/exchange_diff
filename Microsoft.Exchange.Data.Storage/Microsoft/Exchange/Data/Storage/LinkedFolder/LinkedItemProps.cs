using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedItemProps
	{
		public StoreObjectId EntryId { get; private set; }

		public StoreObjectId ParentId { get; private set; }

		public Uri LinkedUri { get; private set; }

		public ExDateTime? LastFullSyncTime { get; private set; }

		public LinkedItemProps(StoreObjectId entryId, StoreObjectId parentId)
		{
			if (entryId == null)
			{
				throw new ArgumentNullException("entryId");
			}
			if (parentId == null)
			{
				throw new ArgumentNullException("parentId");
			}
			this.EntryId = entryId;
			this.ParentId = parentId;
		}

		public LinkedItemProps(StoreObjectId entryId, StoreObjectId parentId, Uri linkedUri) : this(entryId, parentId)
		{
			this.LinkedUri = linkedUri;
		}

		public LinkedItemProps(StoreObjectId entryId, StoreObjectId parentId, Uri linkedUri, ExDateTime? lastFullSyncTime) : this(entryId, parentId, linkedUri)
		{
			this.LastFullSyncTime = lastFullSyncTime;
		}
	}
}
