using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class AllAcceptConstraint : IAllocationConstraint
	{
		public AllAcceptConstraint(params IAllocationConstraint[] constraints)
		{
			if (constraints == null)
			{
				throw new ArgumentNullException("constraints");
			}
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
				if (!constraintValidationResult.Accepted)
				{
					return constraintValidationResult;
				}
			}
			return new ConstraintValidationResult(this, true);
		}

		public IAllocationConstraint CloneForContainer(LoadContainer container)
		{
			IAllocationConstraint[] array = (from x in this.constraints
			select x.CloneForContainer(container)).ToArray<IAllocationConstraint>();
			return new AllAcceptConstraint(array);
		}

		public override string ToString()
		{
			return string.Format("AND({0}).", string.Join<object>(",", this.constraints.Cast<object>()));
		}

		[DataMember]
		private readonly IAllocationConstraint[] constraints;
	}
}
