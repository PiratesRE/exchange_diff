using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal sealed class NeverSetAttributesFilter : PipelineProcessor
	{
		public NeverSetAttributesFilter(IDataProcessor next) : base(next)
		{
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			MultiValuedProperty<AttributeMetadata> multiValuedProperty = (MultiValuedProperty<AttributeMetadata>)propertyBag[ADRecipientSchema.AttributeMetadata];
			if (multiValuedProperty.Count == 0 && ProcessorHelper.IsDeletedObject(propertyBag))
			{
				return true;
			}
			propertyBag.SetIsReadOnly(false);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (AttributeMetadata attributeMetadata in multiValuedProperty)
			{
				hashSet.Add(attributeMetadata.AttributeName);
			}
			foreach (SyncPropertyDefinition property in SyncSchema.Instance.AllBackSyncBaseProperties)
			{
				NeverSetAttributesFilter.FilterProperty(propertyBag, hashSet, property);
			}
			foreach (SyncPropertyDefinition property2 in SyncSchema.Instance.AllBackSyncShadowBaseProperties)
			{
				NeverSetAttributesFilter.FilterProperty(propertyBag, hashSet, property2);
			}
			propertyBag.SetIsReadOnly(true);
			return true;
		}

		private static void FilterProperty(PropertyBag propertyBag, HashSet<string> attributesEverSet, SyncPropertyDefinition property)
		{
			bool flag = false;
			if (property.IsCalculated || !string.IsNullOrEmpty(property.LdapDisplayName))
			{
				if (property.IsCalculated)
				{
					using (IEnumerator<ADPropertyDefinition> enumerator = property.DependentProperties.Cast<ADPropertyDefinition>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ADPropertyDefinition adpropertyDefinition = enumerator.Current;
							if (!string.IsNullOrEmpty(property.LdapDisplayName) && attributesEverSet.Contains(adpropertyDefinition.LdapDisplayName))
							{
								flag = true;
								break;
							}
						}
						goto IL_79;
					}
				}
				flag = attributesEverSet.Contains(property.LdapDisplayName);
				IL_79:
				if (!flag)
				{
					propertyBag.Remove(property);
				}
			}
		}
	}
}
