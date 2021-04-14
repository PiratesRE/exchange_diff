using System;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface IMessageProcessor
	{
		void Process(ISchedulableMessage message);
	}
}
