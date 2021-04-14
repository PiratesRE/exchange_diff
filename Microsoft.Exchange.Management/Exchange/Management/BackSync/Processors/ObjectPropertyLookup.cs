using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class ObjectPropertyLookup : PropertyCache, IPropertyLookup
	{
		public ObjectPropertyLookup(Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getProperties, IDictionary<ADObjectId, ADRawEntry> propertyCache) : base(getProperties, SyncObject.BackSyncProperties.Cast<PropertyDefinition>().ToArray<PropertyDefinition>(), propertyCache)
		{
		}

		public ObjectPropertyLookup(Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getProperties) : this(getProperties, new Dictionary<ADObjectId, ADRawEntry>())
		{
		}

		public override IEnumerable<ADObjectId> GetObjectIds(PropertyBag propertyBag)
		{
			return new ADObjectId[]
			{
				(ADObjectId)propertyBag[ADObjectSchema.Id]
			};
		}
	}
}
