using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class DataBatchEventArgs<T> : EventArgs
	{
		public T DataBatch { get; set; }
	}
}
