using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IBatchDataWriter<T> : IDisposable
	{
		void WriteDataBatch(T dataBatch);
	}
}
