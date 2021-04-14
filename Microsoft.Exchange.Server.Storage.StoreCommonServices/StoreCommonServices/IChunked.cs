using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IChunked
	{
		bool DoChunk(Context context);

		void Dispose(Context context);

		bool MustYield { get; }
	}
}
