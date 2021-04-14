using System;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	public interface IInstantSearchNotificationHandler
	{
		void DeliverInstantSearchPayload(InstantSearchPayloadType instantSearchPayload);
	}
}
