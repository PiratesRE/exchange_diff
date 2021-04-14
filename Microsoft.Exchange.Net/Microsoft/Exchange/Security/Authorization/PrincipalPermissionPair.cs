using System;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authorization
{
	internal class PrincipalPermissionPair
	{
		public PrincipalPermissionPair(SecurityIdentifier principalSid, Permission rights, AccessControlType accessControlType)
		{
			this.principal = principalSid;
			this.permission = rights;
			this.accessControlType = accessControlType;
		}

		public SecurityIdentifier Principal
		{
			get
			{
				return this.principal;
			}
		}

		public Permission Permission
		{
			get
			{
				return this.permission;
			}
		}

		public AccessControlType AccessControlType
		{
			get
			{
				return this.accessControlType;
			}
		}

		private SecurityIdentifier principal;

		private Permission permission;

		private AccessControlType accessControlType;
	}
}
