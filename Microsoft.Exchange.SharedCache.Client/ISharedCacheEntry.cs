using System;

namespace Microsoft.Exchange.SharedCache.Client
{
	public interface ISharedCacheEntry
	{
		void FromByteArray(byte[] bytes);

		byte[] ToByteArray();
	}
}
