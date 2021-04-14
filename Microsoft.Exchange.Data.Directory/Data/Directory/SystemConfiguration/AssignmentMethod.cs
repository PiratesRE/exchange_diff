using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum AssignmentMethod
	{
		None = 0,
		Direct = 1,
		SecurityGroup = 2,
		RoleAssignmentPolicy = 4,
		MailboxPlan = 8,
		RoleGroup = 16,
		ExtraDefaultSids = 32,
		S4U = 50,
		All = 63
	}
}
