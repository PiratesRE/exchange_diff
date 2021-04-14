using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AnyAcceptConstraint : IAllocationConstraint
	{
		public AnyAcceptConstraint(params IAllocationConstraint[] constraints)
		{
			this.constraints = constraints;
		}

		public IAllocationConstraint[] Constraints
		{
			get
			{
				return this.constraints;
			}
		}

		public void ValidateAccepted(LoadEntity entity)
		{
			if (this.Accept(entity))
			{
				return;
			}
			foreach (IAllocationConstraint allocationConstraint in this.constraints)
			{
				allocationConstraint.ValidateAccepted(entity);
			}
		}

		public ConstraintValidationResult Accept(LoadEntity entity)
		{
			foreach (IAllocationConstraint allocationConstraint in this.constraints)
			{
				ConstraintValidationResult constraintValidationResult = allocationConstraint.Accept(entity);
				if (constraintValidationResult.Accepted)
				{
					return constraintValidationResult;
				}
			}
			return new ConstraintValidationResult(this, false);
		}

		public IAllocationConstraint CloneForContainer(LoadContainer container)
		{
			IAllocationConstraint[] array = (from x in this.constraints
			select x.CloneForContainer(container)).ToArray<IAllocationConstraint>();
			return new AnyAcceptConstraint(array);
		}

		public override string ToString()
		{
			return string.Format("OR({0}).", string.Join<object>(",", this.constraints.Cast<object>()));
		}

		[DataMember]
		private readonly IAllocationConstraint[] constraints;
	}
}
