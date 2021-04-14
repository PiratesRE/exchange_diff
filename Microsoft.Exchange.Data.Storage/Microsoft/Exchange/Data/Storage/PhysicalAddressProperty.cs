using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class PhysicalAddressProperty : AtomRuleCompositeProperty
	{
		internal PhysicalAddressProperty(string displayName, NativeStorePropertyDefinition compositeProperty, NativeStorePropertyDefinition streetProperty, NativeStorePropertyDefinition cityProperty, NativeStorePropertyDefinition stateProperty, NativeStorePropertyDefinition postalProperty, NativeStorePropertyDefinition countryProperty) : base(displayName, compositeProperty, new NativeStorePropertyDefinition[]
		{
			streetProperty,
			cityProperty,
			stateProperty,
			postalProperty,
			countryProperty
		})
		{
			this.placeholderCodeToPropDef = PhysicalAddressProperty.CreateMapping(streetProperty, cityProperty, stateProperty, postalProperty, countryProperty);
		}

		private static Dictionary<string, NativeStorePropertyDefinition> CreateMapping(NativeStorePropertyDefinition streetProperty, NativeStorePropertyDefinition cityProperty, NativeStorePropertyDefinition stateProperty, NativeStorePropertyDefinition postalProperty, NativeStorePropertyDefinition countryProperty)
		{
			return Util.AddElements<Dictionary<string, NativeStorePropertyDefinition>, KeyValuePair<string, NativeStorePropertyDefinition>>(new Dictionary<string, NativeStorePropertyDefinition>(), new KeyValuePair<string, NativeStorePropertyDefinition>[]
			{
				new KeyValuePair<string, NativeStorePropertyDefinition>("Street", streetProperty),
				new KeyValuePair<string, NativeStorePropertyDefinition>("City", cityProperty),
				new KeyValuePair<string, NativeStorePropertyDefinition>("State", stateProperty),
				new KeyValuePair<string, NativeStorePropertyDefinition>("Postal", postalProperty),
				new KeyValuePair<string, NativeStorePropertyDefinition>("Region", countryProperty)
			});
		}

		protected override string GenerateCompositePropertyValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			AtomRuleCompositeProperty.FormattedSentenceContext formattedSentenceContext = new AtomRuleCompositeProperty.FormattedSentenceContext(propertyBag, this.placeholderCodeToPropDef);
			FormattedSentence formattedSentence = PhysicalAddressProperty.enUsPattern;
			string text = formattedSentenceContext.ResolvePlaceholder("Region");
			if (text != null)
			{
				Dictionary<string, string> regionMap = PhysicalAddressProperty.addressData.Value.RegionMap;
				text = text.Trim();
				string key;
				if (regionMap.TryGetValue(text, out key))
				{
					Dictionary<string, FormattedSentence> formatMap = PhysicalAddressProperty.addressData.Value.FormatMap;
					FormattedSentence formattedSentence2;
					if (formatMap.TryGetValue(key, out formattedSentence2))
					{
						formattedSentence = formattedSentence2;
					}
				}
			}
			return formattedSentence.Evaluate(formattedSentenceContext);
		}

		private static readonly FormattedSentence enUsPattern = new FormattedSentence("{Street}\r\n<<{City}, {State}> {Postal}>\r\n{Region}");

		private readonly Dictionary<string, NativeStorePropertyDefinition> placeholderCodeToPropDef;

		private static LazilyInitialized<PhysicalAddressData> addressData = new LazilyInitialized<PhysicalAddressData>(() => new PhysicalAddressData());
	}
}
