using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NoLoadContraint : IAllocationConstraint
	{
		public NoLoadContraint(LoadContainer container)
		{
			this.container = container;
		}

		public ConstraintValidationResult Accept(LoadEntity entity)
		{
			return new ConstraintValidationResult(this, false);
		}

		public void ValidateAccepted(LoadEntity entity)
		{
			throw new ContainerCannotReceiveLoadException(this.container.Name);
		}

		public IAllocationConstraint CloneForContainer(LoadContainer container)
		{
			return new NoLoadContraint(container);
		}

		public override string ToString()
		{
			return string.Format("REJECT", new object[0]);
		}

		[DataMember]
		private readonly LoadContainer container;
	}
}
