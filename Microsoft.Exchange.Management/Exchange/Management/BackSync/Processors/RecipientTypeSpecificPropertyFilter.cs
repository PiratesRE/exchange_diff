using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class RecipientTypeSpecificPropertyFilter : PipelineProcessor
	{
		public RecipientTypeSpecificPropertyFilter(IDataProcessor next, IDictionary<ADPropertyDefinition, RecipientTypeDetails> filterMap) : base(next)
		{
			this.filterMap = filterMap;
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			foreach (KeyValuePair<ADPropertyDefinition, RecipientTypeDetails> keyValuePair in this.filterMap)
			{
				ADPropertyDefinition key = keyValuePair.Key;
				RecipientTypeDetails value = keyValuePair.Value;
				if (RecipientTypeSpecificPropertyFilter.ShouldFilterOutProperty(propertyBag, key, value))
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<string, object, RecipientTypeDetails>((long)SyncConfiguration.TraceId, "RecipientTypeSpecificPropertyFilter:: - Removing property {0} from object {1}. It should not be backsycned for recipient of type {2}.", key.Name, propertyBag[ADObjectSchema.Id], value);
					bool isReadOnly = propertyBag.IsReadOnly;
					try
					{
						propertyBag.SetIsReadOnly(false);
						propertyBag.Remove(key);
					}
					finally
					{
						propertyBag.SetIsReadOnly(isReadOnly);
					}
				}
			}
			return true;
		}

		private static bool ShouldFilterOutProperty(PropertyBag propertyBag, ProviderPropertyDefinition property, RecipientTypeDetails allowedTypes)
		{
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)propertyBag[ADRecipientSchema.RecipientTypeDetails];
			return ADDirSyncHelper.ContainsProperty(propertyBag, property) && (allowedTypes & recipientTypeDetails) != recipientTypeDetails;
		}

		private readonly IDictionary<ADPropertyDefinition, RecipientTypeDetails> filterMap;
	}
}
