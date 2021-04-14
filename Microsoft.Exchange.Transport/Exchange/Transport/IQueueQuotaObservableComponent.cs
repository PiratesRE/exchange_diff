using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueQuotaObservableComponent
	{
		event Action<TransportMailItem> OnAcquire;

		event Action<TransportMailItem> OnRelease;
	}
}
