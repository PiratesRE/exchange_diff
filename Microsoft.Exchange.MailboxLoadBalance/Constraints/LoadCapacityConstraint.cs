using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadCapacityConstraint : IAllocationConstraint
	{
		public LoadCapacityConstraint(LoadContainer container)
		{
			this.container = container;
		}

		public ConstraintValidationResult Accept(LoadEntity entity)
		{
			LoadMetric exceededMetric;
			long requestedUnits;
			long availableUnits;
			return new LoadCapacityConstraintValidationResult(this, this.container.AvailableCapacity.SupportsAdditional(entity.ConsumedLoad, out exceededMetric, out requestedUnits, out availableUnits), exceededMetric, availableUnits, requestedUnits);
		}

		public void ValidateAccepted(LoadEntity entity)
		{
			LoadMetric loadMetric;
			long requestedCapacityUnits;
			long availableCapacityUnits;
			if (!this.container.AvailableCapacity.SupportsAdditional(entity.ConsumedLoad, out loadMetric, out requestedCapacityUnits, out availableCapacityUnits))
			{
				throw new NotEnoughDatabaseCapacityPermanentException(this.container.Guid.ToString(), loadMetric.ToString(), requestedCapacityUnits, availableCapacityUnits);
			}
		}

		public IAllocationConstraint CloneForContainer(LoadContainer container)
		{
			return new LoadCapacityConstraint(container);
		}

		public override string ToString()
		{
			return string.Format("HasCapacity", new object[0]);
		}

		[DataMember]
		private readonly LoadContainer container;
	}
}
