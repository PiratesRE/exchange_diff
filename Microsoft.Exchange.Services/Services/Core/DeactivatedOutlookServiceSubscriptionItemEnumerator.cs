using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeactivatedOutlookServiceSubscriptionItemEnumerator : OutlookServiceSubscriptionItemEnumeratorBase
	{
		internal DeactivatedOutlookServiceSubscriptionItemEnumerator(IFolder folder) : this(folder, null, 72U)
		{
		}

		internal DeactivatedOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId) : this(folder, appId, 72U)
		{
		}

		internal DeactivatedOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId, uint deactivationInHours) : this(folder, appId, deactivationInHours, SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		internal DeactivatedOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId, uint deactivationInHours, Unlimited<uint> resultSize) : base(folder, appId, resultSize)
		{
			this.deactivationInHours = deactivationInHours;
		}

		protected override SortBy[] GetSortByConstraint()
		{
			return OutlookServiceSubscriptionItemEnumeratorBase.RefreshTimeUTCAscSortBy;
		}

		protected override bool ShouldStopProcessingItems(IStorePropertyBag item)
		{
			object obj = item.TryGetProperty(OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC);
			ExDateTime? exDateTime = obj as ExDateTime?;
			if (obj is PropertyError || exDateTime == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "DeactivatedOutlookServiceSubscriptionItemEnumerator.ShouldStopProcessingItems: Failed to retrieve SubscriptionRefreshTimeUTC from itemProperty {0}.", item);
				return false;
			}
			if (exDateTime.Value.AddHours(this.deactivationInHours) >= ExDateTime.UtcNow)
			{
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "DeactivatedOutlookServiceSubscriptionItemEnumerator.ShouldStopProcessingItems: Found an deactivated '{0}' subscription {1}:{2} [{3}], stopping processing items.", new object[]
					{
						exDateTime,
						item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.AppId, string.Empty),
						item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.DeviceNotificationId, string.Empty),
						this.deactivationInHours
					});
				}
				return true;
			}
			return false;
		}

		protected override bool ShouldSkipItem(IStorePropertyBag item)
		{
			object obj = item.TryGetProperty(OutlookServiceSubscriptionItemSchema.ExpirationTime);
			ExDateTime? exDateTime = obj as ExDateTime?;
			if (obj is PropertyError || exDateTime == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "DeactivatedOutlookServiceSubscriptionItemEnumerator.ShouldSkipItem: Failed to retrieve ExpirationTime from itemProperty {0}.", item);
				return false;
			}
			if (exDateTime.Value < ExDateTime.UtcNow)
			{
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "DeactivatedOutlookServiceSubscriptionItemEnumerator.ShouldSkipItem: Found an expired '{0}' subscription {1}:{2} [{3}], skipping item.", new object[]
					{
						exDateTime,
						item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.AppId, string.Empty),
						item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.DeviceNotificationId, string.Empty),
						this.deactivationInHours
					});
				}
				return true;
			}
			return false;
		}

		private readonly uint deactivationInHours;
	}
}
