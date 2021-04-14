using System;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal interface ISchedulableItemAdapter<TScheduluableMessage>
	{
		ISchedulableMessage FromItem(TScheduluableMessage item);

		TScheduluableMessage ToItem(ISchedulableMessage message);
	}
}
