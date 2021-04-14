using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal interface ITokenBucket
	{
		uint CurrentBalance { get; }

		ExDateTime NextRecharge { get; }

		bool IsFull { get; }

		bool IsEmpty { get; }

		bool TryTakeToken();
	}
}
