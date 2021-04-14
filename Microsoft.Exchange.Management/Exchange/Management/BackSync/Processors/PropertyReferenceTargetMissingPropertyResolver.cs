using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class PropertyReferenceTargetMissingPropertyResolver : TargetMissingPropertyResolver<PropertyReference>
	{
		public PropertyReferenceTargetMissingPropertyResolver(IDataProcessor next, IPropertyLookup propertyReferenceTargetPropertyLookup) : base(next, propertyReferenceTargetPropertyLookup)
		{
			this.propertyReferenceProperties = new List<PropertyDefinition>();
			this.propertyReferenceProperties.Add(SyncUserSchema.CloudSiteMailboxOwners);
			this.directoryClassLookup = new Dictionary<string, DirectoryObjectClassAddressList>(StringComparer.OrdinalIgnoreCase);
			DirectoryObjectClassAddressList[] array = (DirectoryObjectClassAddressList[])Enum.GetValues(typeof(DirectoryObjectClassAddressList));
			foreach (DirectoryObjectClassAddressList directoryObjectClassAddressList in array)
			{
				this.directoryClassLookup[Enum.GetName(typeof(DirectoryObjectClassAddressList), directoryObjectClassAddressList)] = directoryObjectClassAddressList;
			}
		}

		protected override List<PropertyDefinition> GetResolvingProperties()
		{
			return this.propertyReferenceProperties;
		}

		protected override ADObjectId GetTargetADObjectId(PropertyReference targetPropertyValue)
		{
			return targetPropertyValue.TargetADObjectId;
		}

		protected override bool UpdateTargetMissingProperty(string sourceId, ADPropertyDefinition propertyDefinition, PropertyReference propertyReference, ADRawEntry target)
		{
			string text = (string)target[SyncObjectSchema.ObjectId];
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)target[ADObjectSchema.ObjectClass];
			DirectoryObjectClassAddressList? directoryObjectClassAddressList = null;
			foreach (string key in multiValuedProperty)
			{
				if (this.directoryClassLookup.ContainsKey(key))
				{
					directoryObjectClassAddressList = new DirectoryObjectClassAddressList?(this.directoryClassLookup[key]);
					break;
				}
			}
			if (directoryObjectClassAddressList == null)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string, string, string>((long)SyncConfiguration.TraceId, "PropertyReferenceTargetMissingPropertyResolver:: - Skipping property reference {0} -> {1}. Unsupported target class: {2}", sourceId, text, string.Join(",", multiValuedProperty.ToArray()));
				return false;
			}
			propertyReference.UpdateReferenceData(text, directoryObjectClassAddressList.Value);
			return true;
		}

		private readonly Dictionary<string, DirectoryObjectClassAddressList> directoryClassLookup;

		private readonly List<PropertyDefinition> propertyReferenceProperties;
	}
}
