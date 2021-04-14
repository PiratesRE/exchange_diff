using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal interface IExpiringValue
	{
		DateTime ExpirationTime { get; }

		bool Expired { get; }
	}
}
