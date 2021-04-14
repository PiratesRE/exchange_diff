using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class DynamicDistributionGroupSchema : DistributionGroupBaseSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADDynamicGroupSchema>();
		}

		public static readonly ADPropertyDefinition RecipientFilter = ADDynamicGroupSchema.RecipientFilter;

		public static readonly ADPropertyDefinition LdapRecipientFilter = ADDynamicGroupSchema.LdapRecipientFilter;

		public static readonly ADPropertyDefinition RecipientContainer = ADDynamicGroupSchema.RecipientContainer;

		public static readonly ADPropertyDefinition IncludedRecipients = ADDynamicGroupSchema.IncludedRecipients;

		public static readonly ADPropertyDefinition ConditionalDepartment = ADDynamicGroupSchema.ConditionalDepartment;

		public static readonly ADPropertyDefinition ConditionalCompany = ADDynamicGroupSchema.ConditionalCompany;

		public static readonly ADPropertyDefinition ConditionalStateOrProvince = ADDynamicGroupSchema.ConditionalStateOrProvince;

		public static readonly ADPropertyDefinition RecipientFilterType = ADDynamicGroupSchema.RecipientFilterType;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute1 = ADDynamicGroupSchema.ConditionalCustomAttribute1;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute2 = ADDynamicGroupSchema.ConditionalCustomAttribute2;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute3 = ADDynamicGroupSchema.ConditionalCustomAttribute3;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute4 = ADDynamicGroupSchema.ConditionalCustomAttribute4;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute5 = ADDynamicGroupSchema.ConditionalCustomAttribute5;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute6 = ADDynamicGroupSchema.ConditionalCustomAttribute6;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute7 = ADDynamicGroupSchema.ConditionalCustomAttribute7;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute8 = ADDynamicGroupSchema.ConditionalCustomAttribute8;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute9 = ADDynamicGroupSchema.ConditionalCustomAttribute9;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute10 = ADDynamicGroupSchema.ConditionalCustomAttribute10;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute11 = ADDynamicGroupSchema.ConditionalCustomAttribute11;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute12 = ADDynamicGroupSchema.ConditionalCustomAttribute12;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute13 = ADDynamicGroupSchema.ConditionalCustomAttribute13;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute14 = ADDynamicGroupSchema.ConditionalCustomAttribute14;

		public static readonly ADPropertyDefinition ConditionalCustomAttribute15 = ADDynamicGroupSchema.ConditionalCustomAttribute15;

		public static readonly ADPropertyDefinition ManagedBy = ADDynamicGroupSchema.ManagedBy;

		public static readonly ADPropertyDefinition FilterOnlyManagedBy = ADDynamicGroupSchema.FilterOnlyManagedBy;
	}
}
