using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BufferPools
	{
		public static byte[] GetBuffer(int exactBufferSize, out BufferPool bufferPool)
		{
			bufferPool = null;
			if (exactBufferSize <= 0)
			{
				return null;
			}
			bufferPool = BufferPools.GetBufferPoolOfExactLength(exactBufferSize);
			if (bufferPool == null)
			{
				return new byte[exactBufferSize];
			}
			return bufferPool.Acquire();
		}

		private static BufferPool GetBufferPoolOfExactLength(int exactBufferSize)
		{
			BufferPoolCollection.BufferSize bufferSize;
			if (BufferPools.bufferPoolCollection.TryMatchBufferSize(exactBufferSize, out bufferSize))
			{
				BufferPool bufferPool = BufferPools.bufferPoolCollection.Acquire(bufferSize);
				if (bufferPool.BufferSize == exactBufferSize)
				{
					return bufferPool;
				}
			}
			return null;
		}

		private static readonly BufferPoolCollection bufferPoolCollection = new BufferPoolCollection(false);
	}
}
