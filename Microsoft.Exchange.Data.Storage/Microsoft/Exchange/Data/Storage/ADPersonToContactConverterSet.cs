using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ADPersonToContactConverterSet
	{
		private ADPersonToContactConverterSet(ADPersonToContactConverter[] converters, PropertyDefinition[] personProperties, ADPropertyDefinition[] adProperties)
		{
			this.converters = converters;
			this.personProperties = personProperties;
			this.adProperties = adProperties;
		}

		public static ADPersonToContactConverterSet OrganizationalContactProperties
		{
			get
			{
				return ADPersonToContactConverterSet.organizationalContactProperties;
			}
		}

		public static ADPersonToContactConverterSet PersonSchemaProperties
		{
			get
			{
				return ADPersonToContactConverterSet.personSchemaProperties;
			}
		}

		public ADPropertyDefinition[] ADProperties
		{
			get
			{
				return this.adProperties;
			}
		}

		public PropertyDefinition[] PersonProperties
		{
			get
			{
				return this.personProperties;
			}
		}

		public static ADPersonToContactConverterSet FromPersonProperties(PropertyDefinition[] personProperties, IEnumerable<PropertyDefinition> additionalContactProperties)
		{
			ADPersonToContactConverter[] array = ADPersonToContactConverterSet.GetConverters(personProperties, additionalContactProperties);
			ADPropertyDefinition[] adproperties = ADPersonToContactConverterSet.GetADProperties(array, Array<ADPropertyDefinition>.Empty);
			return new ADPersonToContactConverterSet(array, personProperties, adproperties);
		}

		public static ADPersonToContactConverterSet FromContactProperties(PropertyDefinition[] personProperties, HashSet<PropertyDefinition> contactProperties)
		{
			ADPersonToContactConverter[] array = ADPersonToContactConverterSet.GetConverters(contactProperties);
			ADPropertyDefinition[] adproperties = ADPersonToContactConverterSet.GetADProperties(array, Array<ADPropertyDefinition>.Empty);
			return new ADPersonToContactConverterSet(array, personProperties, adproperties);
		}

		public void Convert(ADRawEntry adObject, IStorePropertyBag contact)
		{
			foreach (ADPersonToContactConverter adpersonToContactConverter in this.converters)
			{
				adpersonToContactConverter.Convert(adObject, contact);
			}
			contact[ContactSchema.PartnerNetworkId] = WellKnownNetworkNames.GAL;
			contact[ContactSchema.GALLinkID] = adObject.Id.ObjectGuid;
			contact[ItemSchema.ParentDisplayName] = string.Empty;
		}

		public IStorePropertyBag Convert(ADRawEntry adObject)
		{
			MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
			IStorePropertyBag storePropertyBag = memoryPropertyBag.AsIStorePropertyBag();
			this.Convert(adObject, storePropertyBag);
			memoryPropertyBag.SetAllPropertiesLoaded();
			return storePropertyBag;
		}

		private static ADPersonToContactConverterSet Create(params ADPropertyDefinition[] additionalADProperties)
		{
			PropertyDefinition[] array = new PropertyDefinition[PersonSchema.Instance.AllProperties.Count];
			PersonSchema.Instance.AllProperties.CopyTo(array, 0);
			ADPersonToContactConverter[] array2 = ADPersonToContactConverterSet.GetConverters(array, null);
			ADPropertyDefinition[] adproperties = ADPersonToContactConverterSet.GetADProperties(array2, additionalADProperties);
			return new ADPersonToContactConverterSet(array2, array, adproperties);
		}

		private static ADPersonToContactConverter[] GetConverters(ICollection<PropertyDefinition> personProperties, IEnumerable<PropertyDefinition> additionalContactProperties)
		{
			new List<ADPersonToContactConverter>(personProperties.Count);
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in personProperties)
			{
				ApplicationAggregatedProperty applicationAggregatedProperty = propertyDefinition as ApplicationAggregatedProperty;
				if (applicationAggregatedProperty != null)
				{
					foreach (PropertyDependency propertyDependency in applicationAggregatedProperty.Dependencies)
					{
						hashSet.Add(propertyDependency.Property);
					}
				}
			}
			if (additionalContactProperties != null)
			{
				foreach (PropertyDefinition item in additionalContactProperties)
				{
					hashSet.Add(item);
				}
			}
			return ADPersonToContactConverterSet.GetConverters(hashSet);
		}

		private static ADPersonToContactConverter[] GetConverters(HashSet<PropertyDefinition> contactProperties)
		{
			List<ADPersonToContactConverter> list = new List<ADPersonToContactConverter>(contactProperties.Count);
			foreach (PropertyDefinition key in contactProperties)
			{
				ADPersonToContactConverter item;
				if (ADPersonToContactConverterSet.contactPropertyToConverterMap.TryGetValue(key, out item))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		private static ADPropertyDefinition[] GetADProperties(ADPersonToContactConverter[] converters, ADPropertyDefinition[] additionalProperties)
		{
			HashSet<ADPropertyDefinition> hashSet = new HashSet<ADPropertyDefinition>();
			foreach (ADPersonToContactConverter adpersonToContactConverter in converters)
			{
				hashSet.UnionWith(adpersonToContactConverter.ADProperties);
			}
			ADPropertyDefinition[] array = new ADPropertyDefinition[hashSet.Count];
			hashSet.CopyTo(array);
			return array;
		}

		private static readonly Dictionary<PropertyDefinition, ADPersonToContactConverter> contactPropertyToConverterMap = ADPersonToContactConverter.AllConverters.ToDictionary((ADPersonToContactConverter item) => item.ContactProperty, (ADPersonToContactConverter item) => item);

		private static readonly ADPersonToContactConverterSet personSchemaProperties = ADPersonToContactConverterSet.Create(new ADPropertyDefinition[0]);

		private static readonly ADPersonToContactConverterSet organizationalContactProperties = ADPersonToContactConverterSet.Create(new ADPropertyDefinition[]
		{
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.LegacyExchangeDN
		});

		private readonly ADPropertyDefinition[] adProperties;

		private readonly PropertyDefinition[] personProperties;

		private readonly ADPersonToContactConverter[] converters;
	}
}
