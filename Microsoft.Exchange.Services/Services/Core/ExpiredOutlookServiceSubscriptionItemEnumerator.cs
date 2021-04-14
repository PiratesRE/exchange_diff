using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ExpiredOutlookServiceSubscriptionItemEnumerator : OutlookServiceSubscriptionItemEnumeratorBase
	{
		internal ExpiredOutlookServiceSubscriptionItemEnumerator(IFolder folder) : this(folder, null)
		{
		}

		internal ExpiredOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId) : this(folder, appId, SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		internal ExpiredOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId, Unlimited<uint> resultSize) : base(folder, appId, resultSize)
		{
		}

		protected override SortBy[] GetSortByConstraint()
		{
			return OutlookServiceSubscriptionItemEnumeratorBase.ExpirationTimeAscSortBy;
		}

		protected override bool ShouldStopProcessingItems(IStorePropertyBag item)
		{
			object obj = item.TryGetProperty(OutlookServiceSubscriptionItemSchema.ExpirationTime);
			ExDateTime? arg = obj as ExDateTime?;
			if (obj is PropertyError || arg == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "ExpiredOutlookServiceSubscriptionItemEnumerator.ShouldStopProcessingItems: Failed to retrieve ExpirationTime from itemProperty {0}.", item);
				return false;
			}
			if (arg.Value >= ExDateTime.UtcNow)
			{
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<ExDateTime?, string, string>((long)this.GetHashCode(), "ExpiredOutlookServiceSubscriptionItemEnumerator.ShouldStopProcessingItems: Found an expired '{0}' subscription {1}:{2}, stopping processing items..", arg, item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.AppId, string.Empty), item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.DeviceNotificationId, string.Empty));
				}
				return true;
			}
			return false;
		}

		protected override bool ShouldSkipItem(IStorePropertyBag item)
		{
			return false;
		}
	}
}
