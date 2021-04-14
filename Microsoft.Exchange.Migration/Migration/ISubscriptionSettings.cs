using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal interface ISubscriptionSettings : IMigrationSerializable
	{
		ExDateTime LastModifiedTime { get; }
	}
}
