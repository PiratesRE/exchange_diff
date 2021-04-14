using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal sealed class BatchLookup : IDataProcessor
	{
		internal BatchLookup(IDataProcessor next, PropertyCache propertyCache)
		{
			this.next = next;
			this.propertyCache = propertyCache;
			this.dataObjects = new List<PropertyBag>();
		}

		public void Process(PropertyBag propertyBag)
		{
			this.dataObjects.Add(propertyBag);
			IEnumerable<ADObjectId> objectIds = this.propertyCache.GetObjectIds(propertyBag);
			foreach (ADObjectId objectId in objectIds)
			{
				this.propertyCache.Enqueue(objectId);
			}
		}

		public void Flush(Func<byte[]> getCookieDelegate, bool moreData)
		{
			this.propertyCache.LookupData();
			this.PropagateData();
			this.next.Flush(getCookieDelegate, moreData);
		}

		private void PropagateData()
		{
			foreach (PropertyBag propertyBag in this.dataObjects)
			{
				this.next.Process(propertyBag);
			}
		}

		private readonly IDataProcessor next;

		private readonly List<PropertyBag> dataObjects;

		private readonly PropertyCache propertyCache;
	}
}
