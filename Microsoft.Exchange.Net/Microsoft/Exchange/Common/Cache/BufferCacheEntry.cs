using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal class BufferCacheEntry
	{
		public BufferCacheEntry(byte[] array, bool ownedByBufferCache)
		{
			this.Buffer = array;
			this.OwnedByBufferCache = ownedByBufferCache;
		}

		public readonly byte[] Buffer;

		public readonly bool OwnedByBufferCache;
	}
}
