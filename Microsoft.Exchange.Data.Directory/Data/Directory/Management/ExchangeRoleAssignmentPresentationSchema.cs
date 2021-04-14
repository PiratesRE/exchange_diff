using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ExchangeRoleAssignmentPresentationSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ExchangeRoleAssignmentSchema>();
		}

		public static readonly ADPropertyDefinition Role = ExchangeRoleAssignmentSchema.Role;

		public static readonly ADPropertyDefinition RoleAssignee = ExchangeRoleAssignmentSchema.User;

		public static readonly ADPropertyDefinition CustomRecipientWriteScope = ExchangeRoleAssignmentSchema.CustomRecipientWriteScope;

		public static readonly ADPropertyDefinition CustomConfigWriteScope = ExchangeRoleAssignmentSchema.CustomConfigWriteScope;

		public static readonly ADPropertyDefinition ExchangeRoleAssignmentFlags = ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags;

		public static readonly ADPropertyDefinition RecipientReadScope = ExchangeRoleAssignmentSchema.RecipientReadScope;

		public static readonly ADPropertyDefinition ConfigReadScope = ExchangeRoleAssignmentSchema.ConfigReadScope;

		public static readonly ADPropertyDefinition RecipientWriteScope = ExchangeRoleAssignmentSchema.RecipientWriteScope;

		public static readonly ADPropertyDefinition ConfigWriteScope = ExchangeRoleAssignmentSchema.ConfigWriteScope;

		public static readonly ADPropertyDefinition RoleAssignmentDelegationType = ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType;

		public static readonly ADPropertyDefinition Enabled = ExchangeRoleAssignmentSchema.Enabled;

		public static readonly ADPropertyDefinition RoleAssigneeType = ExchangeRoleAssignmentSchema.RoleAssigneeType;

		public static readonly ADPropertyDefinition RoleAssigneeName = ExchangeRoleAssignmentSchema.RoleAssigneeName;
	}
}
