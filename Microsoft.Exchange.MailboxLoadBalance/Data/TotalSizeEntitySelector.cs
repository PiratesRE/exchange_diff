using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Constraints;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TotalSizeEntitySelector : BandRandomEntitySelector
	{
		public TotalSizeEntitySelector(Band band, ByteQuantifiedSize targetSize, LoadContainer container, string constraintSetIdentity) : base(band, container, constraintSetIdentity)
		{
			this.totalSize = targetSize;
		}

		public override IEnumerable<LoadEntity> GetEntities(LoadContainer targetContainer)
		{
			IAllocationConstraint allocationConstraint = targetContainer.Constraint ?? new AnyLoadConstraint();
			List<LoadEntity> list = new List<LoadEntity>(base.SourceEntities);
			List<LoadEntity> list2 = new List<LoadEntity>();
			ByteQuantifiedSize value = ByteQuantifiedSize.FromBytes(0UL);
			Random random = new Random();
			while (list.Count > 0 && value < this.totalSize)
			{
				int index = random.Next(list.Count);
				LoadEntity loadEntity = list[index];
				if (allocationConstraint.Accept(loadEntity))
				{
					ByteQuantifiedSize byteQuantifiedSize = value + loadEntity.ConsumedLoad.GetSizeMetric(PhysicalSize.Instance);
					if (byteQuantifiedSize < this.totalSize)
					{
						list2.Add(loadEntity);
						value = byteQuantifiedSize;
					}
				}
				list.RemoveAt(index);
			}
			return list2;
		}

		private readonly ByteQuantifiedSize totalSize;
	}
}
