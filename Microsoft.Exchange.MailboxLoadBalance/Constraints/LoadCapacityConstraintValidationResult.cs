using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[DataContract]
	internal class LoadCapacityConstraintValidationResult : ConstraintValidationResult
	{
		public LoadCapacityConstraintValidationResult(LoadCapacityConstraint constraint, bool accepted, LoadMetric exceededMetric, long availableUnits, long requestedUnits) : base(constraint, accepted)
		{
			this.ExceededMetric = exceededMetric;
			this.AvailableUnits = availableUnits;
			this.RequestedUnits = requestedUnits;
		}

		protected long RequestedUnits { get; set; }

		protected long AvailableUnits { get; set; }

		protected LoadMetric ExceededMetric { get; set; }

		public override string ToString()
		{
			return string.Format("LoadConstraint: '{0}' Accepted: {1}. Requested {2} units of {3} but only had {4} available.", new object[]
			{
				base.Constraint,
				base.Accepted,
				this.RequestedUnits,
				this.ExceededMetric,
				this.AvailableUnits
			});
		}
	}
}
