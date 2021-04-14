using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ITraceCollector<TKey, TParameters>
	{
		void Add(TKey key, TParameters parameters);
	}
}
