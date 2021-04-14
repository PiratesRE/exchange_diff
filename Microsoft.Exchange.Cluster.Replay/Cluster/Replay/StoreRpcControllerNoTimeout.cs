using System;
using Microsoft.Exchange.Data.HA;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class StoreRpcControllerNoTimeout : StoreRpcController
	{
		public StoreRpcControllerNoTimeout(string serverNameOrFqdn) : base(serverNameOrFqdn, null)
		{
		}

		public override TimeSpan ConnectivityTimeout
		{
			get
			{
				return InvokeWithTimeout.InfiniteTimeSpan;
			}
		}
	}
}
