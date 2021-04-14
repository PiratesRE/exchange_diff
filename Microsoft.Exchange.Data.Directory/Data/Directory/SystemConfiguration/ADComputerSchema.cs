using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADComputerSchema : ADNonExchangeObjectSchema
	{
		internal static GetterDelegate OutOfServiceGetterDelegate(ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				return 0 != num;
			};
		}

		internal static QueryFilter IsOutOfServiceFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
			ComparisonOperator comparisonOperator = ComparisonOperator.Equal;
			if ((comparisonFilter.ComparisonOperator == ComparisonOperator.Equal && (bool)comparisonFilter.PropertyValue) || (ComparisonOperator.NotEqual == comparisonFilter.ComparisonOperator && !(bool)comparisonFilter.PropertyValue))
			{
				comparisonOperator = ComparisonOperator.NotEqual;
			}
			return new ComparisonFilter(comparisonOperator, SharedPropertyDefinitions.ProvisioningFlags, 0);
		}

		public static readonly ADPropertyDefinition ServicePrincipalName = new ADPropertyDefinition("ServicePrincipalName", ExchangeObjectVersion.Exchange2003, typeof(string), "servicePrincipalName", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Sid = IADSecurityPrincipalSchema.Sid;

		public static readonly ADPropertyDefinition OperatingSystemVersion = new ADPropertyDefinition("OperatingSystemVersion", ExchangeObjectVersion.Exchange2003, typeof(string), "operatingSystemVersion", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OperatingSystemServicePack = new ADPropertyDefinition("OperatingSystemServicePack", ExchangeObjectVersion.Exchange2003, typeof(string), "operatingSystemServicePack", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UserAccountControl = new ADPropertyDefinition("UserAccountControl", ExchangeObjectVersion.Exchange2003, typeof(UserAccountControlFlags), "userAccountControl", ADPropertyDefinitionFlags.DoNotProvisionalClone, UserAccountControlFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DnsHostName = ADServerSchema.DnsHostName;

		public static readonly ADPropertyDefinition ThrottlingPolicy = new ADPropertyDefinition("ThrottlingPolicy", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchThrottlingPolicyDN", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsOutOfService = new ADPropertyDefinition("IsOutOfService", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(ADComputerSchema.IsOutOfServiceFilterBuilder), ADComputerSchema.OutOfServiceGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags), null, null, null);

		public static readonly ADPropertyDefinition ComponentStates = new ADPropertyDefinition("ComponentStates", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchComponentStates", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MonitoringGroup = new ADPropertyDefinition("MonitoringGroup", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchShadowDisplayName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MonitoringInstalled = new ADPropertyDefinition("MonitoringInstalled", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchCapabilityIdentifiers", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
