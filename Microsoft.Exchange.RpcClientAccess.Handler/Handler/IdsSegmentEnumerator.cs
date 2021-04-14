using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IdsSegmentEnumerator : SegmentEnumerator
	{
		public IdsSegmentEnumerator(StoreObjectId[] objectIds, int segmentSize) : base(segmentSize)
		{
			Util.ThrowOnNullArgument(objectIds, "objectIds");
			this.objectIds = objectIds;
			this.idsIndex = 0;
		}

		public override StoreObjectId[] GetNextBatchIds()
		{
			if (this.idsIndex >= this.objectIds.Length)
			{
				return Array<StoreObjectId>.Empty;
			}
			if (this.idsIndex == 0 && this.objectIds.Length <= base.SegmentSize)
			{
				this.idsIndex += this.objectIds.Length;
				return this.objectIds;
			}
			int val = this.objectIds.Length - this.idsIndex;
			int num = Math.Min(val, base.SegmentSize);
			StoreObjectId[] array = Array<StoreObjectId>.New(num);
			Array.Copy(this.objectIds, this.idsIndex, array, 0, num);
			this.idsIndex += num;
			return array;
		}

		public int Count
		{
			get
			{
				return this.objectIds.Length;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IdsSegmentEnumerator>(this);
		}

		private readonly StoreObjectId[] objectIds;

		private int idsIndex;
	}
}
