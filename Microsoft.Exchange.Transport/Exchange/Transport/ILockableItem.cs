using System;

namespace Microsoft.Exchange.Transport
{
	internal interface ILockableItem : IQueueItem
	{
		AccessToken AccessToken { get; }

		WaitCondition CurrentCondition { get; }

		DateTimeOffset LockExpirationTime { get; set; }

		ThrottlingContext ThrottlingContext { get; set; }

		WaitCondition GetCondition();
	}
}
