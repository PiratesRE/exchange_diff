using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ExchangeRoleAssignmentSchema : ADConfigurationObjectSchema
	{
		internal static object RoleAssigneeNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ExchangeRoleAssignmentSchema.User];
			if (adobjectId == null)
			{
				return string.Empty;
			}
			return adobjectId.Name;
		}

		internal static readonly ExchangeObjectVersion Exchange2009_R3 = new ExchangeObjectVersion(0, 10, 14, 0, 100, 0);

		internal static readonly ExchangeObjectVersion Exchange2009_R4 = new ExchangeObjectVersion(0, 11, 14, 0, 550, 0);

		public static readonly ADPropertyDefinition Role = new ADPropertyDefinition("Role", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchRoleLink", ADPropertyDefinitionFlags.WriteOnce | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition User = new ADPropertyDefinition("User", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchUserLink", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CustomRecipientWriteScope = new ADPropertyDefinition("CustomRecipientWriteScope", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchDomainRestrictionLink", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CustomConfigWriteScope = new ADPropertyDefinition("CustomConfigWriteScope", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchConfigRestrictionLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeRoleAssignmentFlags = new ADPropertyDefinition("ExchangeRoleAssignmentFlags", ExchangeObjectVersion.Exchange2010, typeof(long), "msExchRoleAssignmentFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientReadScope = new ADPropertyDefinition("RecipientReadScope", ExchangeObjectVersion.Exchange2010, typeof(ScopeType), null, ADPropertyDefinitionFlags.Calculated, ScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScopeType)),
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateDomainScope))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags
		}, null, RoleAssignmentFlagsFormat.ScopeTypeGetterDelegate(RoleAssignmentFlagsFormat.Bitfields.RecipientReadScope), RoleAssignmentFlagsFormat.ScopeTypeSetterDelegate(RoleAssignmentFlagsFormat.Bitfields.RecipientReadScope), null, null);

		public static readonly ADPropertyDefinition ConfigReadScope = new ADPropertyDefinition("ConfigReadScope", ExchangeObjectVersion.Exchange2010, typeof(ScopeType), null, ADPropertyDefinitionFlags.Calculated, ScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScopeType)),
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateConfigReadScope))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags
		}, null, RoleAssignmentFlagsFormat.ScopeTypeGetterDelegate(RoleAssignmentFlagsFormat.Bitfields.ConfigReadScope), RoleAssignmentFlagsFormat.ScopeTypeSetterDelegate(RoleAssignmentFlagsFormat.Bitfields.ConfigReadScope), null, null);

		public static readonly ADPropertyDefinition RecipientWriteScope = new ADPropertyDefinition("RecipientWriteScope", ExchangeObjectVersion.Exchange2010, typeof(RecipientWriteScopeType), null, ADPropertyDefinitionFlags.Calculated, RecipientWriteScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(RecipientWriteScopeType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags
		}, new CustomFilterBuilderDelegate(RoleAssignmentFlagsFormat.RecipientWriteScopeFilterBuilder), new GetterDelegate(RoleAssignmentFlagsFormat.RecipientWriteScopeGetter), new SetterDelegate(RoleAssignmentFlagsFormat.RecipientWriteScopeSetter), null, null);

		public static readonly ADPropertyDefinition ConfigWriteScope = new ADPropertyDefinition("ConfigWriteScope", ExchangeObjectVersion.Exchange2010, typeof(ConfigWriteScopeType), null, ADPropertyDefinitionFlags.Calculated, ConfigWriteScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ConfigWriteScopeType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags
		}, new CustomFilterBuilderDelegate(RoleAssignmentFlagsFormat.ConfigWriteScopeFilterBuilder), new GetterDelegate(RoleAssignmentFlagsFormat.ConfigWriteScopeGetter), new SetterDelegate(RoleAssignmentFlagsFormat.ConfigWriteScopeSetter), null, null);

		public static readonly ADPropertyDefinition RoleAssignmentDelegationType = new ADPropertyDefinition("RoleAssignmentDelegationType", ExchangeObjectVersion.Exchange2010, typeof(RoleAssignmentDelegationType), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.WriteOnce, Microsoft.Exchange.Data.Directory.SystemConfiguration.RoleAssignmentDelegationType.Regular, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(RoleAssignmentDelegationType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags
		}, new CustomFilterBuilderDelegate(RoleAssignmentFlagsFormat.RoleAssignmentDelegationFilterBuilder), new GetterDelegate(RoleAssignmentFlagsFormat.RoleAssignmentDelegationTypeGetter), new SetterDelegate(RoleAssignmentFlagsFormat.RoleAssignmentDelegationTypeSetter), null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags
		}, new CustomFilterBuilderDelegate(RoleAssignmentFlagsFormat.RoleAssignmentEnabledFilterBuilder), new GetterDelegate(RoleAssignmentFlagsFormat.EnabledGetter), new SetterDelegate(RoleAssignmentFlagsFormat.EnabledSetter), null, null);

		public static readonly ADPropertyDefinition RoleAssigneeType = new ADPropertyDefinition("RoleAssigneeType", ExchangeObjectVersion.Exchange2010, typeof(RoleAssigneeType), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.SystemConfiguration.RoleAssigneeType.User, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(RoleAssigneeType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags
		}, new CustomFilterBuilderDelegate(RoleAssignmentFlagsFormat.RoleAssigneeTypeFilterBuilder), new GetterDelegate(RoleAssignmentFlagsFormat.RoleAssigneeTypeGetter), new SetterDelegate(RoleAssignmentFlagsFormat.RoleAssigneeTypeSetter), null, null);

		public static readonly ADPropertyDefinition RoleAssigneeName = new ADPropertyDefinition("RoleAssigneeName", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleAssignmentSchema.User
		}, null, new GetterDelegate(ExchangeRoleAssignmentSchema.RoleAssigneeNameGetter), null, null, null);
	}
}
