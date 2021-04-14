using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class RoleGroupSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADGroupSchema>();
		}

		public static readonly ADPropertyDefinition RoleAssignments = ADGroupSchema.RoleAssignments;

		public static readonly ADPropertyDefinition Roles = ADGroupSchema.Roles;

		public static readonly ADPropertyDefinition ManagedBy = ADGroupSchema.ManagedBy;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition Members = ADGroupSchema.Members;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition ForeignGroupSid = ADGroupSchema.ForeignGroupSid;

		public static readonly ADPropertyDefinition LinkedGroup = ADGroupSchema.LinkedGroup;

		public static readonly ADPropertyDefinition RoleGroupType = ADGroupSchema.RoleGroupType;

		public static readonly ADPropertyDefinition RawDescription = ADRecipientSchema.Description;

		public static readonly ADPropertyDefinition Description = ADGroupSchema.RoleGroupDescription;

		public static readonly ADPropertyDefinition RoleGroupTypeId = ADGroupSchema.RoleGroupTypeId;

		public static readonly ADPropertyDefinition LocalizationFlags = ADGroupSchema.LocalizationFlags;

		public static readonly ADPropertyDefinition ExternalDirectoryObjectId = ADRecipientSchema.ExternalDirectoryObjectId;

		public static readonly ADPropertyDefinition LinkedPartnerGroupId = ADGroupSchema.LinkedPartnerGroupId;

		public static readonly ADPropertyDefinition LinkedPartnerOrganizationId = ADGroupSchema.LinkedPartnerOrganizationId;

		public static readonly ADPropertyDefinition RawCapabilities = ADRecipientSchema.RawCapabilities;

		public static readonly ADPropertyDefinition Capabilities = ADRecipientSchema.Capabilities;

		public static readonly ADPropertyDefinition UsnCreated = ADRecipientSchema.UsnCreated;
	}
}
