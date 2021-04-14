using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AttributionSourceId
	{
		private AttributionSourceId(object sourceId)
		{
			this.sourceId = sourceId;
		}

		public static AttributionSourceId CreateFrom(StoreId contactStoreId)
		{
			ArgumentValidator.ThrowIfNull("contactStoreId", contactStoreId);
			return new AttributionSourceId(contactStoreId);
		}

		public static AttributionSourceId CreateFrom(Guid adObjectIdGuid)
		{
			ArgumentValidator.ThrowIfEmpty("adObjectIdGuid", adObjectIdGuid);
			return new AttributionSourceId(adObjectIdGuid);
		}

		public bool IsStoreId
		{
			get
			{
				return this.sourceId is StoreId;
			}
		}

		public bool IsADObjectIdGuid
		{
			get
			{
				return this.sourceId is Guid;
			}
		}

		public StoreId StoreId
		{
			get
			{
				return (StoreId)this.sourceId;
			}
		}

		public Guid ADObjectIdGuid
		{
			get
			{
				return (Guid)this.sourceId;
			}
		}

		private readonly object sourceId;
	}
}
