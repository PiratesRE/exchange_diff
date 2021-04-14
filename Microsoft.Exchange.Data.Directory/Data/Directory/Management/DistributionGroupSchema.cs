using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class DistributionGroupSchema : DistributionGroupBaseSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADGroupSchema>();
		}

		public static readonly ADPropertyDefinition GroupType = ADGroupSchema.GroupType;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition BypassNestedModerationEnabled = ADRecipientSchema.BypassNestedModerationEnabled;

		public static readonly ADPropertyDefinition ManagedBy = ADGroupSchema.ManagedBy;

		public static readonly ADPropertyDefinition MemberJoinRestriction = ADGroupSchema.MemberJoinRestriction;

		public static readonly ADPropertyDefinition MemberDepartRestriction = ADGroupSchema.MemberDepartRestriction;

		public static readonly ADPropertyDefinition Members = ADGroupSchema.Members;
	}
}
