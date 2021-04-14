using System;

namespace Microsoft.Exchange.Net
{
	public class BufferPoolCollection
	{
		public BufferPoolCollection() : this(true)
		{
		}

		public BufferPoolCollection(bool cleanBuffers)
		{
			this.pools = new BufferPool[]
			{
				new BufferPool(1024, cleanBuffers),
				new BufferPool(2048, cleanBuffers),
				new BufferPool(4096, cleanBuffers),
				new BufferPool(8192, cleanBuffers),
				new BufferPool(10240, cleanBuffers),
				new BufferPool(16384, cleanBuffers),
				new BufferPool(20480, cleanBuffers),
				new BufferPool(24576, cleanBuffers),
				new BufferPool(30720, cleanBuffers),
				new BufferPool(32768, cleanBuffers),
				new BufferPool(40960, cleanBuffers),
				new BufferPool(49152, cleanBuffers),
				new BufferPool(51200, cleanBuffers),
				new BufferPool(61440, cleanBuffers),
				new BufferPool(65536, cleanBuffers),
				new BufferPool(71680, cleanBuffers),
				new BufferPool(81920, cleanBuffers),
				new BufferPool(92160, cleanBuffers),
				new BufferPool(98304, cleanBuffers),
				new BufferPool(102400, cleanBuffers),
				new BufferPool(112640, cleanBuffers),
				new BufferPool(122880, cleanBuffers),
				new BufferPool(131072, cleanBuffers),
				new BufferPool(262144, cleanBuffers),
				new BufferPool(524288, cleanBuffers),
				new BufferPool(1048576, cleanBuffers)
			};
			this.cleanBuffers = cleanBuffers;
		}

		public static BufferPoolCollection AutoCleanupCollection
		{
			get
			{
				return BufferPoolCollection.collection;
			}
		}

		public bool CleanBuffersOnRelease
		{
			get
			{
				return this.cleanBuffers;
			}
		}

		public BufferPool Acquire(BufferPoolCollection.BufferSize bufferSize)
		{
			if (bufferSize >= BufferPoolCollection.BufferSize.Size1K && bufferSize < (BufferPoolCollection.BufferSize)this.pools.Length)
			{
				return this.pools[(int)bufferSize];
			}
			throw new ArgumentOutOfRangeException("bufferSize");
		}

		public bool TryMatchBufferSize(int size, out BufferPoolCollection.BufferSize result)
		{
			for (int i = 0; i < this.pools.Length; i++)
			{
				if (this.pools[i].BufferSize >= size)
				{
					result = (BufferPoolCollection.BufferSize)i;
					return true;
				}
			}
			result = BufferPoolCollection.BufferSize.Size1M;
			return false;
		}

		private static BufferPoolCollection collection = new BufferPoolCollection();

		private readonly bool cleanBuffers;

		private BufferPool[] pools;

		public enum BufferSize
		{
			Size1K,
			Size2K,
			Size4K,
			Size8K,
			Size10K,
			Size16K,
			Size20K,
			Size24K,
			Size30K,
			Size32K,
			Size40K,
			Size48K,
			Size50K,
			Size60K,
			Size64K,
			Size70K,
			Size80K,
			Size90K,
			Size96K,
			Size100K,
			Size110K,
			Size120K,
			Size128K,
			Size256K,
			Size512K,
			Size1M
		}
	}
}
