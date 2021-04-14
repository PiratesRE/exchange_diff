using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class TraceDataAggregator<TParameters>
	{
		internal abstract void Add(TParameters parameters);
	}
}
