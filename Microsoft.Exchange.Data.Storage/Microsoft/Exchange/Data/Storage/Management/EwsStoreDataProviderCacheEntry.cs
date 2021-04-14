using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EwsStoreDataProviderCacheEntry
	{
		public virtual bool AlternativeIdCacheEnabled
		{
			get
			{
				return true;
			}
		}

		public bool TryGetItemId(string alternativeId, out ItemId itemId)
		{
			if (!this.AlternativeIdCacheEnabled)
			{
				itemId = null;
				return false;
			}
			return this.alternativeId2Id.TryGetValue(alternativeId, out itemId);
		}

		public void SetItemId(string alternativeId, ItemId itemId)
		{
			if (this.AlternativeIdCacheEnabled && itemId != null)
			{
				this.alternativeId2Id.Add(alternativeId, itemId);
			}
		}

		public virtual void ClearItemCache(EwsStoreObject ewsStoreObject)
		{
			if (this.AlternativeIdCacheEnabled && !string.IsNullOrEmpty(ewsStoreObject.AlternativeId))
			{
				this.alternativeId2Id.Remove(ewsStoreObject.AlternativeId);
			}
		}

		private MruDictionary<string, ItemId> alternativeId2Id = new MruDictionary<string, ItemId>(50, StringComparer.OrdinalIgnoreCase, null);

		public string EwsUrl;

		public int FailedCount;

		public ExDateTime LastDiscoverTime = ExDateTime.MinValue;
	}
}
