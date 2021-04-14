using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal abstract class SegmentEnumerator : BaseObject
	{
		protected SegmentEnumerator(int segmentSize)
		{
			if (segmentSize < 0)
			{
				throw new ArgumentOutOfRangeException("segmentSize");
			}
			this.segmentSize = segmentSize;
		}

		public static int MessageSegmentSize
		{
			get
			{
				return SegmentEnumerator.messageSegmentSize;
			}
		}

		protected int SegmentSize
		{
			get
			{
				return this.segmentSize;
			}
		}

		public static int SetMessageSegmentSize(int messageSegmentSize)
		{
			return Interlocked.Exchange(ref SegmentEnumerator.messageSegmentSize, messageSegmentSize);
		}

		public abstract StoreObjectId[] GetNextBatchIds();

		public const int FolderSegmentSize = 10;

		private static int messageSegmentSize = 1000;

		private readonly int segmentSize;
	}
}
