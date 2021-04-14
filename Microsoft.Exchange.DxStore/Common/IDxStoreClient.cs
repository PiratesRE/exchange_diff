using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public interface IDxStoreClient<T>
	{
		void Initialize(CachedChannelFactory<T> channelFactory, TimeSpan? operationTimeout = null);
	}
}
