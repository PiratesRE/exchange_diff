using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class LinkTargetPropertyLookup : PropertyCache, IPropertyLookup
	{
		public LinkTargetPropertyLookup(Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getProperties, IDictionary<ADObjectId, ADRawEntry> propertyCache) : base(getProperties, SyncObject.BackSyncProperties.Cast<PropertyDefinition>().ToArray<PropertyDefinition>(), propertyCache)
		{
			this.linkProperties = new List<PropertyDefinition>(SyncSchema.Instance.AllBackSyncLinkedProperties.Cast<PropertyDefinition>());
			this.linkProperties.AddRange(SyncSchema.Instance.AllBackSyncShadowLinkedProperties.Cast<PropertyDefinition>());
			this.linkProperties.Add(SyncUserSchema.CloudSiteMailboxOwners);
		}

		public LinkTargetPropertyLookup(Func<ADObjectId[], PropertyDefinition[], Result<ADRawEntry>[]> getProperties) : this(getProperties, new Dictionary<ADObjectId, ADRawEntry>())
		{
		}

		public override IEnumerable<ADObjectId> GetObjectIds(PropertyBag propertyBag)
		{
			HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
			IDictionary<ADPropertyDefinition, object> changedProperties = ADDirSyncHelper.GetChangedProperties(this.linkProperties, propertyBag);
			foreach (KeyValuePair<ADPropertyDefinition, object> keyValuePair in changedProperties)
			{
				if (keyValuePair.Value != null)
				{
					if (keyValuePair.Key.Type == typeof(PropertyReference))
					{
						MultiValuedProperty<PropertyReference> multiValuedProperty = (MultiValuedProperty<PropertyReference>)keyValuePair.Value;
						using (MultiValuedProperty<PropertyReference>.Enumerator enumerator2 = multiValuedProperty.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								PropertyReference propertyReference = enumerator2.Current;
								hashSet.Add(propertyReference.TargetADObjectId);
							}
							continue;
						}
					}
					MultiValuedProperty<SyncLink> multiValuedProperty2 = (MultiValuedProperty<SyncLink>)keyValuePair.Value;
					foreach (SyncLink syncLink in multiValuedProperty2)
					{
						hashSet.Add(syncLink.Link);
					}
				}
			}
			return hashSet;
		}

		private readonly List<PropertyDefinition> linkProperties;
	}
}
