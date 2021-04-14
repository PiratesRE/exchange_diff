using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IBatchDataReader<T>
	{
		event EventHandler<DataBatchEventArgs<T>> DataBatchRead;

		event EventHandler AbortingOnError;

		void StartReading();
	}
}
