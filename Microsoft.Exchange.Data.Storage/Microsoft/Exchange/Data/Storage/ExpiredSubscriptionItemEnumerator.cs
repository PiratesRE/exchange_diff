using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ExpiredSubscriptionItemEnumerator : SubscriptionItemEnumeratorBase
	{
		internal ExpiredSubscriptionItemEnumerator(IFolder folder) : this(folder, 72U)
		{
		}

		internal ExpiredSubscriptionItemEnumerator(IFolder folder, uint expirationInHours) : this(folder, expirationInHours, SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		internal ExpiredSubscriptionItemEnumerator(IFolder folder, uint expirationInHours, Unlimited<uint> resultSize) : base(folder, resultSize)
		{
			this.expirationInHours = expirationInHours;
		}

		protected override SortBy[] GetSortByConstraint()
		{
			return SubscriptionItemEnumeratorBase.RefreshTimeUTCAscSortBy;
		}

		protected override bool ShouldStopProcessingItems(IStorePropertyBag item)
		{
			object obj = item.TryGetProperty(PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC);
			ExDateTime? arg = obj as ExDateTime?;
			if (obj is PropertyError || arg == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "ActiveSubscriptionItemEnumerator.ShouldStopProcessingItems: Failed to retrieve SubscriptionRefreshTimeUTC from itemProperty {0}.", item);
				return false;
			}
			if (arg.Value.AddHours(this.expirationInHours) >= ExDateTime.UtcNow)
			{
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<ExDateTime?, string, uint>((long)this.GetHashCode(), "ExpiredSubscriptionItemEnumerator.ShouldStopProcessingItems: Found an active '{0}' subscription {1} [{2}], stopping processing items.", arg, item.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription, string.Empty), this.expirationInHours);
				}
				return true;
			}
			return false;
		}

		protected override bool ShouldSkipItem(IStorePropertyBag item)
		{
			return false;
		}

		private readonly uint expirationInHours;
	}
}
