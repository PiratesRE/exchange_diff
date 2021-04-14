using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class WindowsGroupSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADGroupSchema>();
		}

		public static readonly ADPropertyDefinition GroupType = ADGroupSchema.GroupType;

		public static readonly ADPropertyDefinition ManagedBy = ADGroupSchema.ManagedBy;

		public static readonly ADPropertyDefinition RecipientType = ADRecipientSchema.RecipientType;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition Sid = ADMailboxRecipientSchema.Sid;

		public static readonly ADPropertyDefinition SidHistory = ADMailboxRecipientSchema.SidHistory;

		public static readonly ADPropertyDefinition SimpleDisplayName = ADRecipientSchema.SimpleDisplayName;

		public static readonly ADPropertyDefinition WindowsEmailAddress = ADRecipientSchema.WindowsEmailAddress;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition Members = ADGroupSchema.Members;

		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;

		public static readonly ADPropertyDefinition MemberOfGroup = ADRecipientSchema.MemberOfGroup;

		public static readonly ADPropertyDefinition OrganizationalUnit = ADRecipientSchema.OrganizationalUnit;

		public static readonly ADPropertyDefinition SeniorityIndex = ADRecipientSchema.HABSeniorityIndex;

		public static readonly ADPropertyDefinition IsHierarchicalGroup = ADGroupSchema.IsOrganizationalGroup;
	}
}
