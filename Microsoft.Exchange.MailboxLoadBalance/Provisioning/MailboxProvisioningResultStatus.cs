using System;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	internal enum MailboxProvisioningResultStatus
	{
		Valid,
		InsufficientCapacity,
		ConstraintCouldNotBeSatisfied
	}
}
