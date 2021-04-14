using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class UserRole : Role
	{
		public UserRole()
		{
		}

		public UserRole(ADObjectId tenantId, ADObjectId userId, string roleGroupName)
		{
			this.TenantId = tenantId;
			this.UserId = userId;
			base.RoleGroupName = roleGroupName;
		}

		public ADObjectId TenantId
		{
			get
			{
				return this[ADObjectSchema.OrganizationalUnitRoot] as ADObjectId;
			}
			internal set
			{
				this[ADObjectSchema.OrganizationalUnitRoot] = value;
			}
		}

		public ADObjectId UserId
		{
			get
			{
				return (ADObjectId)this[UserRole.UserIdDef];
			}
			internal set
			{
				this[UserRole.UserIdDef] = value;
			}
		}

		public bool IsCannedRole
		{
			get
			{
				return (bool)this[UserRole.IsCannedRoleDef];
			}
		}

		public bool IsUserRole
		{
			get
			{
				return (bool)this[UserRole.IsUserRoleDef];
			}
		}

		internal static readonly HygienePropertyDefinition UserIdDef = new HygienePropertyDefinition("userId", typeof(ADObjectId));

		internal static readonly HygienePropertyDefinition IsCannedRoleDef = new HygienePropertyDefinition("isCannedRole", typeof(bool));

		internal static readonly HygienePropertyDefinition IsUserRoleDef = new HygienePropertyDefinition("isUserRole", typeof(bool));
	}
}
