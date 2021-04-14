using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class CountryListSchema : ADConfigurationObjectSchema
	{
		internal static void CountriesSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = null;
			MultiValuedProperty<CountryInfo> multiValuedProperty2 = (MultiValuedProperty<CountryInfo>)value;
			if (multiValuedProperty2 != null)
			{
				multiValuedProperty = new MultiValuedProperty<string>();
				foreach (CountryInfo countryInfo in multiValuedProperty2)
				{
					multiValuedProperty.Add(countryInfo.Name);
				}
			}
			propertyBag[CountryListSchema.RawCountries] = multiValuedProperty;
		}

		internal static object CountriesGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<CountryInfo> multiValuedProperty = null;
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[CountryListSchema.RawCountries];
			if (multiValuedProperty2 != null)
			{
				multiValuedProperty = new MultiValuedProperty<CountryInfo>();
				foreach (string name in multiValuedProperty2)
				{
					multiValuedProperty.Add(CountryInfo.Parse(name));
				}
			}
			return multiValuedProperty;
		}

		internal static readonly ExchangeObjectVersion ObjectVersion = ExchangeObjectVersion.Exchange2010;

		public static readonly ADPropertyDefinition RawCountries = new ADPropertyDefinition("RawCountries", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchCountries", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Countries = new ADPropertyDefinition("Countries", ExchangeObjectVersion.Exchange2010, typeof(CountryInfo), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			CountryListSchema.RawCountries
		}, null, new GetterDelegate(CountryListSchema.CountriesGetter), new SetterDelegate(CountryListSchema.CountriesSetter), null, null);
	}
}
