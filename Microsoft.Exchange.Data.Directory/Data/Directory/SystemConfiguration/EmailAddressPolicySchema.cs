using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class EmailAddressPolicySchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition RecipientFilter = new ADPropertyDefinition("RecipientFilter", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchQueryFilter", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 2048)
		}, null, null);

		public static readonly ADPropertyDefinition LdapRecipientFilter = SharedPropertyDefinitions.LdapRecipientFilter;

		public static readonly ADPropertyDefinition PurportedSearchUI = SharedPropertyDefinitions.PurportedSearchUI;

		public static readonly ADPropertyDefinition RecipientFilterMetadata = SharedPropertyDefinitions.RecipientFilterMetadata;

		public static readonly ADPropertyDefinition RawEnabledEmailAddressTemplates = new ADPropertyDefinition("EnabledEmailAddressTemplates", ExchangeObjectVersion.Exchange2003, typeof(ProxyAddressTemplate), "gatewayProxy", ADPropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1123)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DisabledEmailAddressTemplates = new ADPropertyDefinition("DisabledEmailAddressTemplates", ExchangeObjectVersion.Exchange2003, typeof(ProxyAddressTemplate), "disabledGatewayProxy", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NonAuthoritativeDomains = new ADPropertyDefinition("NonAuthoritativeDomains", ExchangeObjectVersion.Exchange2003, typeof(ProxyAddressTemplate), "msExchNonAuthoritativeDomains", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Priority = new ADPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2003, typeof(EmailAddressPolicyPriority), "msExchPolicyOrder", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, (EmailAddressPolicyPriority)0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EmailAddressPolicyPriority>((EmailAddressPolicyPriority)EmailAddressPolicyPriority.LenientHighestPriorityValue, EmailAddressPolicyPriority.Lowest)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PolicyOptionListValue = new ADPropertyDefinition("PolicyOptionListValue", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchPolicyOptionList", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchPolicyEnabled", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminDescription = new ADPropertyDefinition("AdminDescription", ExchangeObjectVersion.Exchange2003, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LastUpdatedRecipientFilter = SharedPropertyDefinitions.LastUpdatedRecipientFilter;

		public static readonly ADPropertyDefinition RecipientFilterFlags = new ADPropertyDefinition("RecipientFilterFlags", ExchangeObjectVersion.Exchange2007, typeof(RecipientFilterableObjectFlags), "msExchRecipientFilterFlags", ADPropertyDefinitionFlags.PersistDefaultValue, RecipientFilterableObjectFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientContainer = SharedPropertyDefinitions.RecipientContainer;

		public static readonly ADPropertyDefinition IncludedRecipients = new ADPropertyDefinition("IncludedRecipients", ExchangeObjectVersion.Exchange2007, typeof(WellKnownRecipientType?), null, ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new NullableWellKnownRecipientTypeConstraint()
		}, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.IncludeRecipientGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.IncludeRecipientSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalDepartment = new ADPropertyDefinition("ConditionalDepartment", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 64)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.DepartmentGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalDepartment), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.DepartmentSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCompany = new ADPropertyDefinition("ConditionalCompany", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CompanyGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCompany), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CompanySetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalStateOrProvince = new ADPropertyDefinition("ConditionalStateOrProvince", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 128)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.StateOrProvinceGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalStateOrProvince), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.StateOrProvinceSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute1 = new ADPropertyDefinition("ConditionalCustomAttribute1", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute1), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute1, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute2 = new ADPropertyDefinition("ConditionalCustomAttribute2", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute2), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute2, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute3 = new ADPropertyDefinition("ConditionalCustomAttribute3", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute3), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute3, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute4 = new ADPropertyDefinition("ConditionalCustomAttribute4", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute4), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute4, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute5 = new ADPropertyDefinition("ConditionalCustomAttribute5", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute5), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute5, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute6 = new ADPropertyDefinition("ConditionalCustomAttribute6", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute6), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute6, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute7 = new ADPropertyDefinition("ConditionalCustomAttribute7", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute7), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute7, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute8 = new ADPropertyDefinition("ConditionalCustomAttribute8", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute8), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute8, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute9 = new ADPropertyDefinition("ConditionalCustomAttribute9", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute9), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute9, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute10 = new ADPropertyDefinition("ConditionalCustomAttribute10", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute10), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute10, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute11 = new ADPropertyDefinition("ConditionalCustomAttribute11", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute11), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute11, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute12 = new ADPropertyDefinition("ConditionalCustomAttribute12", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute12), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute12, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute13 = new ADPropertyDefinition("ConditionalCustomAttribute13", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute13), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute13, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute14 = new ADPropertyDefinition("ConditionalCustomAttribute14", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute14), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute14, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute15 = new ADPropertyDefinition("ConditionalCustomAttribute15", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter,
			EmailAddressPolicySchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute15), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.ConditionalCustomAttribute15, EmailAddressPolicySchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition RecipientFilterType = new ADPropertyDefinition("RecipientFilterType", ExchangeObjectVersion.Exchange2007, typeof(WellKnownRecipientFilterType), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, WellKnownRecipientFilterType.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterMetadata,
			EmailAddressPolicySchema.RecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.RecipientFilterTypeGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter), null, null, null);

		public static readonly ADPropertyDefinition EnabledEmailAddressTemplates = new ADPropertyDefinition("EnabledEmailAddressTemplates", ExchangeObjectVersion.Exchange2003, typeof(ProxyAddressTemplate), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new EnabledEmailAddressTemplatesConstraint()
		}, new ProviderPropertyDefinition[]
		{
			EmailAddressPolicySchema.RawEnabledEmailAddressTemplates
		}, null, new GetterDelegate(EmailAddressPolicy.EnabledEmailAddressTemplatesGetter), new SetterDelegate(EmailAddressPolicy.EnabledEmailAddressTemplatesSetter), null, null);

		public static readonly ADPropertyDefinition EnabledPrimarySMTPAddressTemplate = new ADPropertyDefinition("EnabledPrimarySMTPAddressTemplate", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			EmailAddressPolicySchema.RawEnabledEmailAddressTemplates
		}, null, new GetterDelegate(EmailAddressPolicy.EnabledPrimarySMTPAddressTemplateGetter), new SetterDelegate(EmailAddressPolicy.EnabledPrimarySMTPAddressTemplateSetter), null, null);

		public static readonly ADPropertyDefinition RecipientFilterApplied = new ADPropertyDefinition("RecipientFilterApplied", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.RecipientFilterFlags
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.RecipientFilterAppliedGetter(propertyBag, EmailAddressPolicySchema.RecipientFilterFlags), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.RecipientFilterAppliedSetter(value, propertyBag, EmailAddressPolicySchema.RecipientFilterFlags);
		}, null, null);

		public static readonly ADPropertyDefinition HasEmailAddressSetting = new ADPropertyDefinition("HasEmailAddressSetting", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			EmailAddressPolicySchema.PolicyOptionListValue
		}, null, (IPropertyBag propertyBag) => EmailAddressPolicy.IsOfPolicyType((MultiValuedProperty<byte[]>)propertyBag[EmailAddressPolicySchema.PolicyOptionListValue], EmailAddressPolicy.PolicyGuid), null, null, null);

		public static readonly ADPropertyDefinition HasMailboxManagerSetting = new ADPropertyDefinition("HasMailboxManagerSetting", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			EmailAddressPolicySchema.PolicyOptionListValue
		}, null, (IPropertyBag propertyBag) => EmailAddressPolicy.IsOfPolicyType((MultiValuedProperty<byte[]>)propertyBag[EmailAddressPolicySchema.PolicyOptionListValue], EmailAddressPolicy.MailboxSettingPolicyGuid), null, null, null);
	}
}
