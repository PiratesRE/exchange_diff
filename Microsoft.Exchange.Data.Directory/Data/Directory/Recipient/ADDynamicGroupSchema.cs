using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADDynamicGroupSchema : ADRecipientSchema
	{
		public static readonly ADPropertyDefinition ExpansionServer = IADDistributionListSchema.ExpansionServer;

		public static readonly ADPropertyDefinition PurportedSearchUI = SharedPropertyDefinitions.PurportedSearchUI;

		public static readonly ADPropertyDefinition RecipientFilterMetadata = SharedPropertyDefinitions.RecipientFilterMetadata;

		public static readonly ADPropertyDefinition RecipientContainer = new ADPropertyDefinition("RecipientContainer", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchDynamicDLBaseDN", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition LdapRecipientFilter = new ADPropertyDefinition("LdapRecipientFilter", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchDynamicDLFilter", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition RecipientFilter = SharedPropertyDefinitions.RecipientFilter;

		public static readonly ADPropertyDefinition ManagedBy = IADDistributionListSchema.RawManagedBy;

		public static readonly ADPropertyDefinition Members = IADDistributionListSchema.Members;

		public static readonly ADPropertyDefinition ReportToManagerEnabled = IADDistributionListSchema.ReportToManagerEnabled;

		public static readonly ADPropertyDefinition ReportToOriginatorEnabled = IADDistributionListSchema.ReportToOriginatorEnabled;

		public static readonly ADPropertyDefinition SendDeliveryReportsTo = IADDistributionListSchema.SendDeliveryReportsTo;

		public static readonly ADPropertyDefinition SendOofMessageToOriginatorEnabled = IADDistributionListSchema.SendOofMessageToOriginatorEnabled;

		public static readonly ADPropertyDefinition IncludedRecipients = new ADPropertyDefinition("IncludedRecipients", ExchangeObjectVersion.Exchange2007, typeof(WellKnownRecipientType?), null, ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new NullableWellKnownRecipientTypeConstraint()
		}, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, new CustomFilterBuilderDelegate(ADDynamicGroup.IncludeRecipientFilterBuilder), (IPropertyBag propertyBag) => RecipientFilterHelper.IncludeRecipientGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.IncludeRecipientSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalDepartment = new ADPropertyDefinition("ConditionalDepartment", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 64)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.DepartmentGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalDepartment), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.DepartmentSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCompany = new ADPropertyDefinition("ConditionalCompany", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CompanyGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCompany), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CompanySetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalStateOrProvince = new ADPropertyDefinition("ConditionalStateOrProvince", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 128)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.StateOrProvinceGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalStateOrProvince), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.StateOrProvinceSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition RecipientFilterType = new ADPropertyDefinition("RecipientFilterType", ExchangeObjectVersion.Exchange2007, typeof(WellKnownRecipientFilterType), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, WellKnownRecipientFilterType.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.RecipientFilterTypeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter), null, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute1 = new ADPropertyDefinition("ConditionalCustomAttribute1", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute1), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute1, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute2 = new ADPropertyDefinition("ConditionalCustomAttribute2", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute2), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute2, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute3 = new ADPropertyDefinition("ConditionalCustomAttribute3", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute3), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute3, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute4 = new ADPropertyDefinition("ConditionalCustomAttribute4", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute4), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute4, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute5 = new ADPropertyDefinition("ConditionalCustomAttribute5", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute5), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute5, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute6 = new ADPropertyDefinition("ConditionalCustomAttribute6", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute6), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute6, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute7 = new ADPropertyDefinition("ConditionalCustomAttribute7", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute7), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute7, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute8 = new ADPropertyDefinition("ConditionalCustomAttribute8", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute8), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute8, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute9 = new ADPropertyDefinition("ConditionalCustomAttribute9", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute9), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute9, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute10 = new ADPropertyDefinition("ConditionalCustomAttribute10", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute10), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute10, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute11 = new ADPropertyDefinition("ConditionalCustomAttribute11", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute11), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute11, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute12 = new ADPropertyDefinition("ConditionalCustomAttribute12", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute12), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute12, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute13 = new ADPropertyDefinition("ConditionalCustomAttribute13", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute13), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute13, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute14 = new ADPropertyDefinition("ConditionalCustomAttribute14", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute14), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute14, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute15 = new ADPropertyDefinition("ConditionalCustomAttribute15", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			ADDynamicGroupSchema.RecipientFilterMetadata,
			ADDynamicGroupSchema.RecipientFilter,
			ADDynamicGroupSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute15), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.ConditionalCustomAttribute15, ADDynamicGroupSchema.LdapRecipientFilter, true);
		}, null, null);

		public static readonly ADPropertyDefinition FilterOnlyManagedBy = ADGroupSchema.ManagedBy;

		public static readonly ADPropertyDefinition GroupMemberCount = new ADPropertyDefinition("GroupMemberCount", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchGroupMemberCount", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GroupExternalMemberCount = new ADPropertyDefinition("GroupExternalMemberCount", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchGroupExternalMemberCount", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
