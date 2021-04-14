using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RoleAssigneeType
	{
		User,
		SecurityGroup = 2,
		RoleAssignmentPolicy = 4,
		MailboxPlan = 6,
		ForeignSecurityPrincipal = 8,
		RoleGroup = 10,
		PartnerLinkedRoleGroup,
		LinkedRoleGroup,
		Computer = 14
	}
}
