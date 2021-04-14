using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IComponentData
	{
		bool DoCleanup(Context context);
	}
}
