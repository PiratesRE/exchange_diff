using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	public interface IDistributedStoreChangeNotify : IDisposable
	{
		void Start();
	}
}
