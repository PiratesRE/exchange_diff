using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BatchCapacityProjection : ICapacityProjection
	{
		public BatchCapacityProjection(int numberOfMailboxes)
		{
			this.numberOfMailboxes = numberOfMailboxes;
		}

		public BatchCapacityDatum GetCapacity()
		{
			return new BatchCapacityDatum
			{
				MaximumNumberOfMailboxes = this.numberOfMailboxes
			};
		}

		private readonly int numberOfMailboxes;
	}
}
