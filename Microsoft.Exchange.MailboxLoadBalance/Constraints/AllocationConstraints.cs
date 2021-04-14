using System;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	internal static class AllocationConstraints
	{
		public static IAllocationConstraint Or(params IAllocationConstraint[] constraints)
		{
			return new AnyAcceptConstraint(constraints);
		}

		public static IAllocationConstraint And(params IAllocationConstraint[] constraints)
		{
			return new AllAcceptConstraint(constraints);
		}
	}
}
