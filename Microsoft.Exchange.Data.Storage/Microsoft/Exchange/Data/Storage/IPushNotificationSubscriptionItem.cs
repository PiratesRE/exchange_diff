using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPushNotificationSubscriptionItem : IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		string SubscriptionId { get; set; }

		ExDateTime LastUpdateTimeUTC { get; set; }

		string SerializedNotificationSubscription { get; set; }
	}
}
