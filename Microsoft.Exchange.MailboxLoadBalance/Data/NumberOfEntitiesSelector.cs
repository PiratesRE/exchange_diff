using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Constraints;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NumberOfEntitiesSelector : BandRandomEntitySelector
	{
		public NumberOfEntitiesSelector(Band band, long numberOfEntities, LoadContainer sourceContainer, string constraintSetIdentity) : base(band, sourceContainer, constraintSetIdentity)
		{
			this.totalNumberOfEntities = numberOfEntities;
		}

		public override IEnumerable<LoadEntity> GetEntities(LoadContainer targetContainer)
		{
			List<LoadEntity> list = new List<LoadEntity>(base.SourceEntities);
			List<LoadEntity> list2 = new List<LoadEntity>();
			IAllocationConstraint allocationConstraint = targetContainer.Constraint ?? new AnyLoadConstraint();
			Random random = new Random();
			while (list.Count > 0 && (long)list2.Count < this.totalNumberOfEntities)
			{
				int index = random.Next(list.Count);
				LoadEntity loadEntity = list[index];
				if (allocationConstraint.Accept(loadEntity))
				{
					list2.Add(loadEntity);
				}
				list.RemoveAt(index);
			}
			return list2;
		}

		private readonly long totalNumberOfEntities;
	}
}
