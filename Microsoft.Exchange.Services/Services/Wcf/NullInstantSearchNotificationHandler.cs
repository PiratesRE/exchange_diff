using System;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class NullInstantSearchNotificationHandler : IInstantSearchNotificationHandler
	{
		public void DeliverInstantSearchPayload(InstantSearchPayloadType instantSearchPayload)
		{
		}
	}
}
