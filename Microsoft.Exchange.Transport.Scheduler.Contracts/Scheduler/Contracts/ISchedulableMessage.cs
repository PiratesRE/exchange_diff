using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal interface ISchedulableMessage
	{
		TransportMessageId Id { get; }

		MessageEnvelope MessageEnvelope { get; }

		DateTime SubmitTime { get; }

		IEnumerable<IMessageScope> Scopes { get; }
	}
}
