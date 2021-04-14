using System;

namespace Microsoft.Exchange.Rpc
{
	internal class AsyncBufferPools
	{
		private AsyncBufferPools(int bufferSize)
		{
			this.privateBufferPool = new AsyncBufferPool(bufferSize);
		}

		private byte[] Aquire()
		{
			return this.privateBufferPool.Acquire();
		}

		private void Release(byte[] buffer)
		{
			this.privateBufferPool.Release(buffer);
		}

		private int BufferSize
		{
			get
			{
				return this.privateBufferPool.BufferSize;
			}
		}

		public static byte[] GetBuffer(int requestedBufferSize)
		{
			if (requestedBufferSize < 0)
			{
				throw new FailRpcException("Buffer size cannot be negative", -2147024809);
			}
			if (requestedBufferSize == 0)
			{
				return AsyncBufferPools.EmptyBuffer;
			}
			int num = 0;
			if (0 < AsyncBufferPools.BufferPools.Length)
			{
				do
				{
					AsyncBufferPools asyncBufferPools = AsyncBufferPools.BufferPools[num];
					if (requestedBufferSize <= asyncBufferPools.privateBufferPool.BufferSize)
					{
						goto IL_4D;
					}
					num++;
				}
				while (num < AsyncBufferPools.BufferPools.Length);
				goto IL_5F;
				IL_4D:
				return AsyncBufferPools.BufferPools[num].privateBufferPool.Acquire();
			}
			IL_5F:
			throw new FailRpcException("Buffer size too large", -2147024809);
		}

		public static void ReleaseBuffer(byte[] buffer)
		{
			if (buffer != null && buffer.Length != 0)
			{
				int num = 0;
				if (0 < AsyncBufferPools.BufferPools.Length)
				{
					do
					{
						int bufferSize = AsyncBufferPools.BufferPools[num].privateBufferPool.BufferSize;
						if (buffer.Length == bufferSize)
						{
							goto IL_38;
						}
						num++;
					}
					while (num < AsyncBufferPools.BufferPools.Length);
					goto IL_4C;
					IL_38:
					AsyncBufferPools.BufferPools[num].privateBufferPool.Release(buffer);
					return;
				}
				IL_4C:
				throw new ArgumentException("buffer being released doesn't match any buffer pool length");
			}
		}

		private static readonly byte[] EmptyBuffer = new byte[0];

		private static readonly AsyncBufferPools[] BufferPools = new AsyncBufferPools[]
		{
			new AsyncBufferPools(EmsmdbConstants.ExtendedBufferHeaderSize + 1024),
			new AsyncBufferPools(EmsmdbConstants.MaxExtendedAuxBufferSize),
			new AsyncBufferPools(EmsmdbConstants.MaxExtendedRopBufferSize),
			new AsyncBufferPools(EmsmdbConstants.MaxOutlookChainedExtendedRopBufferSize),
			new AsyncBufferPools(EmsmdbConstants.MaxMapiHttpChainedOutlookPayloadSize),
			new AsyncBufferPools(EmsmdbConstants.MaxChainedExtendedRopBufferSize),
			new AsyncBufferPools(EmsmdbConstants.MaxMapiHttpChainedPayloadSize)
		};

		private readonly AsyncBufferPool privateBufferPool;

		public static readonly int MaxBufferSize = EmsmdbConstants.MaxMapiHttpChainedPayloadSize;
	}
}
