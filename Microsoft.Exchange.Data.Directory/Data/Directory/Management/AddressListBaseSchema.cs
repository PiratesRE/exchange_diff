using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal abstract class AddressListBaseSchema : ADPresentationSchema
	{
		public new static readonly ADPropertyDefinition Name = AddressBookBaseSchema.Name;

		public static readonly ADPropertyDefinition RecipientFilter = AddressBookBaseSchema.RecipientFilter;

		public static readonly ADPropertyDefinition LdapRecipientFilter = AddressBookBaseSchema.LdapRecipientFilter;

		public static readonly ADPropertyDefinition IncludedRecipients = AddressBookBaseSchema.IncludedRecipients;

		public static readonly ADPropertyDefinition ConditionalDepartment = AddressBookBaseSchema.ConditionalDepartment;

		public static readonly ADPropertyDefinition ConditionalCompany = AddressBookBaseSchema.ConditionalCompany;

		public static readonly ADPropertyDefinition ConditionalStateOrProvince = AddressBookBaseSchema.ConditionalStateOrProvince;

		public static readonly ADPropertyDefinition RecipientFilterType = AddressBookBaseSchema.RecipientFilterType;

		public static readonly ADPropertyDefinition LastUpdatedRecipientFilter = AddressBookBaseSchema.LastUpdatedRecipientFilter;

		public static readonly ADPropertyDefinition RecipientFilterApplied = AddressBookBaseSchema.RecipientFilterApplied;

		public static readonly ADPropertyDefinition RecipientContainer = AddressBookBaseSchema.RecipientContainer;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute1 = AddressBookBaseSchema.ConditionalCustomAttribute1;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute2 = AddressBookBaseSchema.ConditionalCustomAttribute2;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute3 = AddressBookBaseSchema.ConditionalCustomAttribute3;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute4 = AddressBookBaseSchema.ConditionalCustomAttribute4;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute5 = AddressBookBaseSchema.ConditionalCustomAttribute5;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute6 = AddressBookBaseSchema.ConditionalCustomAttribute6;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute7 = AddressBookBaseSchema.ConditionalCustomAttribute7;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute8 = AddressBookBaseSchema.ConditionalCustomAttribute8;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute9 = AddressBookBaseSchema.ConditionalCustomAttribute9;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute10 = AddressBookBaseSchema.ConditionalCustomAttribute10;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute11 = AddressBookBaseSchema.ConditionalCustomAttribute11;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute12 = AddressBookBaseSchema.ConditionalCustomAttribute12;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute13 = AddressBookBaseSchema.ConditionalCustomAttribute13;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute14 = AddressBookBaseSchema.ConditionalCustomAttribute14;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute15 = AddressBookBaseSchema.ConditionalCustomAttribute15;
	}
}
