using System;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal class LocalDeliveryConnectionHandler
	{
		public static void HandleConnection(NextHopConnection connection)
		{
			IStoreDriver storeDriver;
			if (!Components.TryGetStoreDriver(out storeDriver))
			{
				ExTraceGlobals.QueuingTracer.TraceError(0L, "No store driver found");
				return;
			}
			ExTraceGlobals.QueuingTracer.TraceDebug(0L, "Invoking the store driver");
			Components.StoreDriver.DoLocalDelivery(connection);
		}
	}
}
