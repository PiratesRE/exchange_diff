using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IDigestCollector
	{
		void LogActivity(ResourceDigestStats activity);
	}
}
