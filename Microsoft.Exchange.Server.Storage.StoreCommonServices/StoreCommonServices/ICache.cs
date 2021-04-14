using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ICache
	{
		bool FlushAllDirtyEntries(Context context);
	}
}
