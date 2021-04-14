using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class AnyLoadConstraint : IAllocationConstraint
	{
		public ConstraintValidationResult Accept(LoadEntity entity)
		{
			return new ConstraintValidationResult(this, true);
		}

		public void ValidateAccepted(LoadEntity entity)
		{
		}

		public IAllocationConstraint CloneForContainer(LoadContainer container)
		{
			return new AnyLoadConstraint();
		}

		public override string ToString()
		{
			return string.Format("ACCEPT", new object[0]);
		}
	}
}
