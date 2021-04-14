using System;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SimpleInstantSearchNotificationHandler : IInstantSearchNotificationHandler
	{
		public SimpleInstantSearchNotificationHandler(Action<InstantSearchPayloadType> searchPayloadCallback)
		{
			this.searchPayloadCallback = searchPayloadCallback;
		}

		public void DeliverInstantSearchPayload(InstantSearchPayloadType instantSearchPayload)
		{
			this.searchPayloadCallback(instantSearchPayload);
		}

		private Action<InstantSearchPayloadType> searchPayloadCallback;
	}
}
