using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IOutlookServiceSubscriptionItem : IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		string SubscriptionId { get; set; }

		ExDateTime LastUpdateTimeUTC { get; set; }

		string PackageId { get; set; }

		string AppId { get; set; }

		string DeviceNotificationId { get; set; }

		ExDateTime ExpirationTime { get; set; }

		bool LockScreen { get; set; }
	}
}
