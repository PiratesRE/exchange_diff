using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ManagementScopeSchema : ADConfigurationObjectSchema
	{
		internal static readonly ExchangeObjectVersion ExchangeManagementScope2010_SPVersion = new ExchangeObjectVersion(1, 10, 14, 1, 90, 0);

		public static readonly ADPropertyDefinition RecipientRoot = new ADPropertyDefinition("RecipientRoot", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchScopeRoot", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Filter = new ADPropertyDefinition("Filter", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchQueryFilter", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ScopeRestrictionFlags = new ADPropertyDefinition("ScopeRestrictionFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchScopeFlags", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.WriteOnce, 3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ScopeRestrictionType = new ADPropertyDefinition("ScopeRestrictionType", ExchangeObjectVersion.Exchange2010, typeof(ScopeRestrictionType), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.WriteOnce, Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.RecipientScope, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScopeRestrictionType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ManagementScopeSchema.ScopeRestrictionFlags
		}, new CustomFilterBuilderDelegate(ScopeFlagsFormat.ScopeRestrictionTypeFilterBuilder), new GetterDelegate(ScopeFlagsFormat.ScopeRestrictionTypeGetter), new SetterDelegate(ScopeFlagsFormat.ScopeRestrictionTypeSetter), null, null);

		public static readonly ADPropertyDefinition Exclusive = new ADPropertyDefinition("Exclusive", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.WriteOnce, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ManagementScopeSchema.ScopeRestrictionFlags
		}, new CustomFilterBuilderDelegate(ScopeFlagsFormat.ExclusiveTypeFilterBuilder), new GetterDelegate(ScopeFlagsFormat.ExclusiveTypeGetter), new SetterDelegate(ScopeFlagsFormat.ExclusiveTypeSetter), null, null);

		public static readonly ADPropertyDefinition RecipientWriteScopeBL = new ADPropertyDefinition("RecipientWriteScopeBL", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchDomainRestrictionBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigWriteScopeBL = new ADPropertyDefinition("ConfigWriteScopeBL", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchConfigRestrictionBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrganizationFilter = new ADPropertyDefinition("OrganizationFilter", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ManagementScopeSchema.Filter
		}, null, ScopeFlagsFormat.FilterGetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.PartnerDelegatedTenantScope), ScopeFlagsFormat.FilterSetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.PartnerDelegatedTenantScope), null, null);

		public static readonly ADPropertyDefinition RecipientFilter = new ADPropertyDefinition("RecipientFilter", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ManagementScopeSchema.Filter
		}, null, ScopeFlagsFormat.FilterGetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.RecipientScope), ScopeFlagsFormat.FilterSetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.RecipientScope), null, null);

		public static readonly ADPropertyDefinition ServerFilter = new ADPropertyDefinition("ServerFilter", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ManagementScopeSchema.Filter
		}, null, ScopeFlagsFormat.FilterGetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.ServerScope), ScopeFlagsFormat.FilterSetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.ServerScope), null, null);

		public static readonly ADPropertyDefinition DatabaseFilter = new ADPropertyDefinition("DatabaseFilter", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ManagementScopeSchema.Filter
		}, null, ScopeFlagsFormat.FilterGetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.DatabaseScope), ScopeFlagsFormat.FilterSetterDelegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ScopeRestrictionType.DatabaseScope), null, null);
	}
}
