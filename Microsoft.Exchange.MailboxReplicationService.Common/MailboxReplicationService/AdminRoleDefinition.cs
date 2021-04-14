using System;
using System.Security.Principal;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct AdminRoleDefinition
	{
		public AdminRoleDefinition(SecurityIdentifier sid, string roleName)
		{
			this.Sid = sid;
			this.RoleName = roleName;
		}

		public SecurityIdentifier Sid;

		public string RoleName;
	}
}
