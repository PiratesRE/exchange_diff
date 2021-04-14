using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ActiveOutlookServiceSubscriptionItemEnumerator : OutlookServiceSubscriptionItemEnumeratorBase
	{
		public ActiveOutlookServiceSubscriptionItemEnumerator(IFolder folder) : this(folder, null, 72U)
		{
		}

		public ActiveOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId) : this(folder, appId, 72U)
		{
		}

		public ActiveOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId, uint deactivationInHours) : this(folder, appId, deactivationInHours, SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		public ActiveOutlookServiceSubscriptionItemEnumerator(IFolder folder, string appId, uint deactivationInHours, Unlimited<uint> resultSize) : base(folder, appId, resultSize)
		{
			this.deactivationInHours = deactivationInHours;
		}

		protected override SortBy[] GetSortByConstraint()
		{
			return OutlookServiceSubscriptionItemEnumeratorBase.RefreshTimeUTCDescSortBy;
		}

		protected override bool ShouldStopProcessingItems(IStorePropertyBag item)
		{
			object obj = item.TryGetProperty(OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC);
			ExDateTime? exDateTime = obj as ExDateTime?;
			if (obj is PropertyError || exDateTime == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "ActiveOutlookServiceSubscriptionItemEnumerator.ShouldStopProcessingItems: Failed to retrieve SubscriptionRefreshTimeUTC from itemProperty {0}.", item);
				return true;
			}
			if (exDateTime.Value.AddHours(this.deactivationInHours) < ExDateTime.UtcNow)
			{
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "ActiveOutlookServiceSubscriptionItemEnumerator.ShouldStopProcessingItems: Found a deactivated '{0}' subscription {1}:{2} [{3}], stopping processing items..", new object[]
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
			string valueOrDefault = item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.SubscriptionId, null);
			if (valueOrDefault == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "ActiveOutlookServiceSubscriptionItemEnumerator.ShouldSkipItem: Failed to retrieve SubscriptionId from itemProperty {0}.", item);
				return false;
			}
			if (this.ids.Contains(valueOrDefault))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "ActiveOutlookServiceSubscriptionItemEnumerator.ShouldSkipItem: Replicated item found on ActiveOutlookServiceSubscriptionEnumerator '{0}'.", valueOrDefault);
				return true;
			}
			this.ids.Add(valueOrDefault);
			object obj = item.TryGetProperty(OutlookServiceSubscriptionItemSchema.ExpirationTime);
			ExDateTime? exDateTime = obj as ExDateTime?;
			if (obj is PropertyError || exDateTime == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "ActiveSubscriptionItemEnumerator.ShouldSkipItem: Failed to retrieve ExpirationTime from itemProperty {0}.", item);
				return true;
			}
			if (exDateTime.Value < ExDateTime.UtcNow)
			{
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "ActiveSubscriptionItemEnumerator.ShouldSkipItem: Found an expired '{0}' subscription {1}:{2} [{3}], skipping item.", new object[]
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

		private HashSet<string> ids = new HashSet<string>();
	}
}
