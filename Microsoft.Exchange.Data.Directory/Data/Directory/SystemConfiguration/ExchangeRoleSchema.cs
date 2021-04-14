using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ExchangeRoleSchema : ADConfigurationObjectSchema
	{
		internal static readonly ExchangeObjectVersion Exchange2009_R3 = new ExchangeObjectVersion(0, 11, 14, 0, 418, 0);

		internal static readonly ExchangeObjectVersion Exchange2009_R4DF5 = new ExchangeObjectVersion(0, 12, 14, 0, 451, 0);

		internal static readonly ExchangeObjectVersion CurrentRoleVersion = ExchangeRoleSchema.Exchange2009_R4DF5;

		internal static readonly ADPropertyDefinition RoleEntries = new ADPropertyDefinition("RoleEntries", ExchangeRoleSchema.Exchange2009_R3, typeof(RoleEntry), "msExchRoleEntries", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		internal static readonly ADPropertyDefinition InternalDownlevelRoleEntries = new ADPropertyDefinition("InternalDownlevelRoleEntries", ExchangeObjectVersion.Exchange2010, typeof(RoleEntry), "msExchRoleEntriesExt", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition RoleFlags = new ADPropertyDefinition("RoleFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchRoleFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoleType = new ADPropertyDefinition("RoleType", ExchangeRoleSchema.Exchange2009_R3, typeof(RoleType), "msExchRoleType", ADPropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Directory.SystemConfiguration.RoleType.MyBaseOptions, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ImplicitRecipientReadScope = new ADPropertyDefinition("ImplicitRecipientReadScope", ExchangeRoleSchema.Exchange2009_R3, typeof(ScopeType), null, ADPropertyDefinitionFlags.Calculated, ScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScopeType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RoleFlags
		}, null, new GetterDelegate(RoleFlagsFormat.ScopeTypeGetterDelegate(ScopeLocation.RecipientRead).Invoke), new SetterDelegate(RoleFlagsFormat.ScopeTypeSetterDelegate(ScopeLocation.RecipientRead).Invoke), null, null);

		public static readonly ADPropertyDefinition ImplicitRecipientWriteScope = new ADPropertyDefinition("ImplicitRecipientWriteScope", ExchangeRoleSchema.Exchange2009_R3, typeof(ScopeType), null, ADPropertyDefinitionFlags.Calculated, ScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScopeType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RoleFlags
		}, null, new GetterDelegate(RoleFlagsFormat.ScopeTypeGetterDelegate(ScopeLocation.RecipientWrite).Invoke), new SetterDelegate(RoleFlagsFormat.ScopeTypeSetterDelegate(ScopeLocation.RecipientWrite).Invoke), null, null);

		public static readonly ADPropertyDefinition ImplicitConfigReadScope = new ADPropertyDefinition("ImplicitConfigReadScope", ExchangeRoleSchema.Exchange2009_R3, typeof(ScopeType), null, ADPropertyDefinitionFlags.Calculated, ScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScopeType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RoleFlags
		}, null, new GetterDelegate(RoleFlagsFormat.ScopeTypeGetterDelegate(ScopeLocation.ConfigRead).Invoke), new SetterDelegate(RoleFlagsFormat.ScopeTypeSetterDelegate(ScopeLocation.ConfigRead).Invoke), null, null);

		public static readonly ADPropertyDefinition ImplicitConfigWriteScope = new ADPropertyDefinition("ImplicitConfigWriteScope", ExchangeRoleSchema.Exchange2009_R3, typeof(ScopeType), null, ADPropertyDefinitionFlags.Calculated, ScopeType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScopeType))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RoleFlags
		}, null, new GetterDelegate(RoleFlagsFormat.ScopeTypeGetterDelegate(ScopeLocation.ConfigWrite).Invoke), new SetterDelegate(RoleFlagsFormat.ScopeTypeSetterDelegate(ScopeLocation.ConfigWrite).Invoke), null, null);

		public static readonly ADPropertyDefinition RoleState = new ADPropertyDefinition("RoleState", ExchangeRoleSchema.Exchange2009_R3, typeof(RoleState), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.SystemConfiguration.RoleState.Usable, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RoleFlags
		}, null, new GetterDelegate(RoleFlagsFormat.GetRoleState), new SetterDelegate(RoleFlagsFormat.SetRoleState), null, null);

		public static readonly ADPropertyDefinition IsDeprecated = new ADPropertyDefinition("IsDeprecated", ExchangeRoleSchema.Exchange2009_R3, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RoleFlags
		}, null, new GetterDelegate(RoleFlagsFormat.GetIsDeprecated), null, null, null);

		public static readonly ADPropertyDefinition RoleAssignments = new ADPropertyDefinition("RoleAssignments", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchRoleBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HasDownlevelData = new ADPropertyDefinition("HasDownlevelData", ExchangeRoleSchema.Exchange2009_R3, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.ExchangeVersion,
			ExchangeRoleSchema.InternalDownlevelRoleEntries
		}, null, new GetterDelegate(ExchangeRole.HasDownlevelDataGetter), null, null, null);

		public static readonly ADPropertyDefinition IsRootRole = new ADPropertyDefinition("IsRootRole", ExchangeRoleSchema.Exchange2009_R3, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ExchangeRole.IsRootRoleGetter), null, null, null);

		public static readonly ADPropertyDefinition IsEndUserRole = new ADPropertyDefinition("IsEndUserRole", ExchangeRoleSchema.Exchange2009_R3, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RoleFlags
		}, new CustomFilterBuilderDelegate(RoleFlagsFormat.IsEndUserRoleFilterBuilder), new GetterDelegate(RoleFlagsFormat.GetIsEndUserRole), new SetterDelegate(RoleFlagsFormat.SetIsEndUserRole), null, null);

		public static readonly ADPropertyDefinition RawMailboxPlanIndex = new ADPropertyDefinition("RawMailboxPlanIndex", ExchangeRoleSchema.Exchange2009_R3, typeof(string), "msExchMailboxPlanType", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxPlanIndex = new ADPropertyDefinition("MailboxPlanIndex", ExchangeRoleSchema.Exchange2009_R3, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 256)
		}, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ExchangeRoleSchema.RawMailboxPlanIndex,
			ADObjectSchema.ConfigurationUnit,
			ExchangeRoleSchema.RoleType
		}, null, new GetterDelegate(ExchangeRole.MailboxPlanIndexGetter), new SetterDelegate(ExchangeRole.MailboxPlanIndexSetter), null, null);

		public static readonly ADPropertyDefinition RawDescription = SharedPropertyDefinitions.RawDescription;

		public static readonly ADPropertyDefinition Description = new ADPropertyDefinition("Description", ExchangeRoleSchema.Exchange2009_R3, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, new ProviderPropertyDefinition[]
		{
			ExchangeRoleSchema.RawDescription,
			ExchangeRoleSchema.RoleType,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ExchangeRole.DescriptionGetter), new SetterDelegate(ExchangeRole.DescriptionSetter), null, null);

		public static readonly ADPropertyDefinition Parent = new ADPropertyDefinition("Parent", ExchangeRoleSchema.Exchange2009_R3, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ExchangeRole.ParentGetter), null, null, null);
	}
}
