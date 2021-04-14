using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[DataContract]
	internal class ConstraintValidationResult
	{
		public ConstraintValidationResult(IAllocationConstraint constraint, bool accepted)
		{
			this.Constraint = constraint;
			this.Accepted = accepted;
		}

		[DataMember]
		public IAllocationConstraint Constraint { get; private set; }

		[DataMember]
		public bool Accepted { get; private set; }

		public static implicit operator bool(ConstraintValidationResult result)
		{
			return result != null && result.Accepted;
		}

		public override string ToString()
		{
			return string.Format("Constraint: '{0}' Accepted: {1}.", this.Constraint, this.Accepted);
		}
	}
}
