using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AddressBookBaseSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition SimpleDisplayName = SharedPropertyDefinitions.SimpleDisplayName;

		public static readonly ADPropertyDefinition RecipientFilter = SharedPropertyDefinitions.RecipientFilter;

		public static readonly ADPropertyDefinition LdapRecipientFilter = SharedPropertyDefinitions.LdapRecipientFilter;

		public static readonly ADPropertyDefinition PurportedSearchUI = SharedPropertyDefinitions.PurportedSearchUI;

		public static readonly ADPropertyDefinition RecipientFilterMetadata = SharedPropertyDefinitions.RecipientFilterMetadata;

		public new static readonly ADPropertyDefinition SystemFlags = new ADPropertyDefinition("SystemFlags", ExchangeObjectVersion.Exchange2003, typeof(SystemFlagsEnum), "systemFlags", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.WriteOnce, SystemFlagsEnum.Movable | SystemFlagsEnum.Renamable, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsSystemAddressList = new ADPropertyDefinition("IsSystemAddressList", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchSystemAddressList", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProvisioningFlags = new ADPropertyDefinition("ProvisioningFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchProvisioningFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsModernGroupsAddressList = new ADPropertyDefinition("IsModernGroupsAddressList", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AddressBookBaseSchema.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(1, AddressBookBaseSchema.ProvisioningFlags), ADObject.FlagSetterDelegate(1, AddressBookBaseSchema.ProvisioningFlags), null, null);

		public static readonly ADPropertyDefinition IsInSystemAddressListContainer = new ADPropertyDefinition("IsInSystemAddressListContainer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.ConfigurationUnit
		}, null, new GetterDelegate(AddressBookBase.IsInSystemAddressListContainerGetter), null, null, null);

		public static readonly ADPropertyDefinition Base64Guid = new ADPropertyDefinition("Base64Guid", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(AddressBookBase.Base64GuidGetter), null, null, null);

		public static readonly ADPropertyDefinition Container = new ADPropertyDefinition("Container", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.ConfigurationUnit
		}, null, new GetterDelegate(AddressBookBase.ContainerGetter), null, null, null);

		public static readonly ADPropertyDefinition Path = new ADPropertyDefinition("Path", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(AddressBookBase.PathGetter), null, null, null);

		public static readonly ADPropertyDefinition IsTopContainer = new ADPropertyDefinition("IsTopContainer", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.ConfigurationUnit
		}, null, new GetterDelegate(AddressBookBase.IsTopContainerGetter), null, null, null);

		public static readonly ADPropertyDefinition IsGlobalAddressList = new ADPropertyDefinition("IsGlobalAddressList", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.ConfigurationUnit
		}, null, new GetterDelegate(AddressBookBase.IsGlobalAddressListGetter), null, null, null);

		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.MandatoryDisplayName;

		public static readonly ADPropertyDefinition LastUpdatedRecipientFilter = SharedPropertyDefinitions.LastUpdatedRecipientFilter;

		public static readonly ADPropertyDefinition RecipientFilterFlags = new ADPropertyDefinition("RecipientFilterFlags", ExchangeObjectVersion.Exchange2007, typeof(RecipientFilterableObjectFlags), "msExchRecipientFilterFlags", ADPropertyDefinitionFlags.PersistDefaultValue, RecipientFilterableObjectFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientContainer = SharedPropertyDefinitions.RecipientContainer;

		public static readonly ADPropertyDefinition RecipientFilterApplied = new ADPropertyDefinition("RecipientFilterApplied", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterFlags
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.RecipientFilterAppliedGetter(propertyBag, AddressBookBaseSchema.RecipientFilterFlags), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.RecipientFilterAppliedSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterFlags);
		}, null, null);

		public static readonly ADPropertyDefinition IsDefaultGlobalAddressList = new ADPropertyDefinition("IsDefaultGlobalAddressList", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.ExchangeVersion,
			ADObjectSchema.ConfigurationUnit,
			AddressBookBaseSchema.PurportedSearchUI,
			AddressBookBaseSchema.LdapRecipientFilter
		}, new CustomFilterBuilderDelegate(AddressBookBase.IsDefaultGlobalAddressListFilterBuilder), new GetterDelegate(AddressBookBase.IsDefaultGlobalAddressListGetter), null, null, null);

		public new static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2003, typeof(string), "name", ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new ADObjectNameStringLengthConstraint(1, 64),
			new ContainingNonWhitespaceConstraint(),
			new ADObjectNameCharacterConstraint(new char[]
			{
				'\\',
				'\0',
				'\n'
			})
		}, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ADObject.NameGetter), new SetterDelegate(ADObject.NameSetter), null, null);

		public static readonly ADPropertyDefinition IncludedRecipients = new ADPropertyDefinition("IncludedRecipients", ExchangeObjectVersion.Exchange2007, typeof(WellKnownRecipientType?), null, ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new NullableWellKnownRecipientTypeConstraint()
		}, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.IncludeRecipientGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.IncludeRecipientSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalDepartment = new ADPropertyDefinition("ConditionalDepartment", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 64)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.DepartmentGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalDepartment), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.DepartmentSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCompany = new ADPropertyDefinition("ConditionalCompany", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CompanyGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCompany), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CompanySetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalStateOrProvince = new ADPropertyDefinition("ConditionalStateOrProvince", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 128)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.StateOrProvinceGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalStateOrProvince), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.StateOrProvinceSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition RecipientFilterType = new ADPropertyDefinition("RecipientFilterType", ExchangeObjectVersion.Exchange2007, typeof(WellKnownRecipientFilterType), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, WellKnownRecipientFilterType.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.RecipientFilterTypeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter), null, null, null);

		public static readonly ADPropertyDefinition AssociatedAddressBookPoliciesForAddressLists = new ADPropertyDefinition("AssociatedAddressBookPoliciesForAddressLists", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAddressListsBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AssociatedAddressBookPoliciesForGAL = new ADPropertyDefinition("AssociatedAddressBookPoliciesForGAL", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchGlobalAddressListBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AssociatedAddressBookPoliciesForAllRoomList = new ADPropertyDefinition("AssociatedAddressBookPoliciesForAllRoomList", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAllRoomListBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute1 = new ADPropertyDefinition("ConditionalCustomAttribute1", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute1), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute1, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute2 = new ADPropertyDefinition("ConditionalCustomAttribute2", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute2), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute2, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute3 = new ADPropertyDefinition("ConditionalCustomAttribute3", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute3), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute3, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute4 = new ADPropertyDefinition("ConditionalCustomAttribute4", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute4), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute4, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute5 = new ADPropertyDefinition("ConditionalCustomAttribute5", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute5), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute5, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute6 = new ADPropertyDefinition("ConditionalCustomAttribute6", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute6), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute6, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute7 = new ADPropertyDefinition("ConditionalCustomAttribute7", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute7), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute7, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute8 = new ADPropertyDefinition("ConditionalCustomAttribute8", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute8), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute8, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute9 = new ADPropertyDefinition("ConditionalCustomAttribute9", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute9), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute9, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute10 = new ADPropertyDefinition("ConditionalCustomAttribute10", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute10), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute10, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute11 = new ADPropertyDefinition("ConditionalCustomAttribute11", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute11), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute11, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute12 = new ADPropertyDefinition("ConditionalCustomAttribute12", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute12), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute12, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute13 = new ADPropertyDefinition("ConditionalCustomAttribute13", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute13), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute13, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute14 = new ADPropertyDefinition("ConditionalCustomAttribute14", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute14), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute14, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);

		public static readonly ADPropertyDefinition ConditionalCustomAttribute15 = new ADPropertyDefinition("ConditionalCustomAttribute15", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 2048)
		}, new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.RecipientFilterMetadata,
			AddressBookBaseSchema.RecipientFilter,
			AddressBookBaseSchema.LdapRecipientFilter
		}, null, (IPropertyBag propertyBag) => RecipientFilterHelper.CustomAttributeGetter(propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute15), delegate(object value, IPropertyBag propertyBag)
		{
			RecipientFilterHelper.CustomAttributeSetter(value, propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.ConditionalCustomAttribute15, AddressBookBaseSchema.LdapRecipientFilter, false);
		}, null, null);
	}
}
