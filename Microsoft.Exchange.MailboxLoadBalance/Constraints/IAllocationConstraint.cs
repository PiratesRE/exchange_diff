using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IAllocationConstraint
	{
		ConstraintValidationResult Accept(LoadEntity entity);

		void ValidateAccepted(LoadEntity entity);

		IAllocationConstraint CloneForContainer(LoadContainer container);
	}
}
