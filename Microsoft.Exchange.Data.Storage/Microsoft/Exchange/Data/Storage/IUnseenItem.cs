using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IUnseenItem
	{
		StoreId Id { get; }

		ExDateTime ReceivedTime { get; }
	}
}
