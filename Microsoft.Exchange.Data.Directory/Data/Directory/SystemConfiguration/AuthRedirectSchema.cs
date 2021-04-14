using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AuthRedirectSchema : ADNonExchangeObjectSchema
	{
		public static object AuthSchemeGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[AuthRedirectSchema.Keywords];
			string value = multiValuedProperty.Find((string x) => !string.Equals(x, AuthRedirect.AuthRedirectKeywords, StringComparison.OrdinalIgnoreCase));
			AuthScheme authScheme = Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthScheme.Unknown;
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					authScheme = (AuthScheme)Enum.Parse(typeof(AuthScheme), value, true);
				}
				catch (ArgumentException)
				{
				}
			}
			return authScheme;
		}

		private static void AuthSchemeSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[AuthRedirectSchema.Keywords];
			multiValuedProperty.Clear();
			string item = value.ToString();
			multiValuedProperty.Add(AuthRedirect.AuthRedirectKeywords);
			multiValuedProperty.Add(item);
		}

		private static QueryFilter AuthSchemeFilterBuilder(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return new AndFilter(new QueryFilter[]
				{
					AuthRedirect.AuthRedirectKeywordsFilter,
					new ComparisonFilter(ComparisonOperator.Equal, AuthRedirectSchema.Keywords, comparisonFilter.PropertyValue.ToString())
				});
			case ComparisonOperator.NotEqual:
				return new AndFilter(new QueryFilter[]
				{
					AuthRedirect.AuthRedirectKeywordsFilter,
					new ComparisonFilter(ComparisonOperator.NotEqual, AuthRedirectSchema.Keywords, comparisonFilter.PropertyValue.ToString())
				});
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
		}

		public new static readonly ADPropertyDefinition ExchangeVersion = new ADPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2003, typeof(ExchangeObjectVersion), null, ADPropertyDefinitionFlags.TaskPopulated | ADPropertyDefinitionFlags.DoNotProvisionalClone, ExchangeObjectVersion.Exchange2003, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Keywords = new ADPropertyDefinition("Keywords", ExchangeObjectVersion.Exchange2003, typeof(string), "keywords", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetUrl = new ADPropertyDefinition("TargetUrl", ExchangeObjectVersion.Exchange2003, typeof(string), "serviceBindingInformation", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuthScheme = new ADPropertyDefinition("AuthScheme", ExchangeObjectVersion.Exchange2003, typeof(AuthScheme), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.SystemConfiguration.AuthScheme.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AuthRedirectSchema.Keywords
		}, new CustomFilterBuilderDelegate(AuthRedirectSchema.AuthSchemeFilterBuilder), new GetterDelegate(AuthRedirectSchema.AuthSchemeGetter), new SetterDelegate(AuthRedirectSchema.AuthSchemeSetter), null, null);
	}
}
