using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal abstract class PropertyCache
	{
		protected PropertyCache(Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getProperties, PropertyDefinition[] properties) : this(getProperties, properties, new Dictionary<ADObjectId, ADRawEntry>())
		{
		}

		protected PropertyCache(Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getProperties, PropertyDefinition[] properties, IDictionary<ADObjectId, ADRawEntry> propertyCache)
		{
			this.properties = properties;
			this.getProperties = getProperties;
			this.propertyCache = propertyCache;
			this.lookupQueue = new HashSet<ADObjectId>();
		}

		public ADRawEntry GetProperties(ADObjectId objectId)
		{
			if (!this.propertyCache.ContainsKey(objectId))
			{
				this.LookupData(new ADObjectId[]
				{
					objectId
				});
			}
			return this.propertyCache[objectId];
		}

		public void Enqueue(ADObjectId objectId)
		{
			if (!this.IsInCache(objectId) && !this.IsQueuedForLookup(objectId))
			{
				this.EnqueueForLookup(objectId);
			}
		}

		public void LookupData()
		{
			ADObjectId[] ids = this.lookupQueue.ToArray<ADObjectId>();
			this.LookupData(ids);
		}

		public abstract IEnumerable<ADObjectId> GetObjectIds(PropertyBag propertyBag);

		protected virtual bool MeetsAdditionalCriteria(ADRawEntry entry)
		{
			return true;
		}

		private void LookupData(ADObjectId[] ids)
		{
			Result<ADRawEntry>[] array = this.getProperties(ids, this.properties);
			for (int i = 0; i < ids.Length; i++)
			{
				ADRawEntry adrawEntry = array[i].Data;
				if (adrawEntry != null && !this.MeetsAdditionalCriteria(adrawEntry))
				{
					adrawEntry = null;
				}
				this.propertyCache[ids[i]] = adrawEntry;
			}
		}

		private void EnqueueForLookup(ADObjectId objectId)
		{
			this.lookupQueue.Add(objectId);
		}

		private bool IsInCache(ADObjectId objectId)
		{
			return this.propertyCache.ContainsKey(objectId);
		}

		private bool IsQueuedForLookup(ADObjectId objectId)
		{
			return this.lookupQueue.Contains(objectId);
		}

		private readonly Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getProperties;

		private readonly IDictionary<ADObjectId, ADRawEntry> propertyCache;

		private readonly HashSet<ADObjectId> lookupQueue;

		private readonly PropertyDefinition[] properties;
	}
}
