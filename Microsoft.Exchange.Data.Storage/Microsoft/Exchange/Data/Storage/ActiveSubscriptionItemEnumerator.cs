using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ActiveSubscriptionItemEnumerator : SubscriptionItemEnumeratorBase
	{
		public ActiveSubscriptionItemEnumerator(IFolder folder) : this(folder, 72U)
		{
		}

		public ActiveSubscriptionItemEnumerator(IFolder folder, uint expirationInHours) : this(folder, expirationInHours, SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		public ActiveSubscriptionItemEnumerator(IFolder folder, uint expirationInHours, Unlimited<uint> resultSize) : base(folder, resultSize)
		{
			this.expirationInHours = expirationInHours;
		}

		protected override SortBy[] GetSortByConstraint()
		{
			return SubscriptionItemEnumeratorBase.RefreshTimeUTCDescSortBy;
		}

		protected override bool ShouldStopProcessingItems(IStorePropertyBag item)
		{
			object obj = item.TryGetProperty(PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC);
			ExDateTime? arg = obj as ExDateTime?;
			if (obj is PropertyError || arg == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "ActiveSubscriptionItemEnumerator.ShouldStopProcessingItems: Failed to retrieve SubscriptionRefreshTimeUTC from itemProperty {0}.", item);
				return true;
			}
			if (arg.Value.AddHours(this.expirationInHours) < ExDateTime.UtcNow)
			{
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<ExDateTime?, string, uint>((long)this.GetHashCode(), "ActiveSubscriptionItemEnumerator.ShouldStopProcessingItems: Found an expired '{0}' subscription {1} [{2}], stopping processing items.", arg, item.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription, string.Empty), this.expirationInHours);
				}
				return true;
			}
			return false;
		}

		protected override bool ShouldSkipItem(IStorePropertyBag item)
		{
			string valueOrDefault = item.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SubscriptionId, null);
			if (valueOrDefault == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "ActiveSubscriptionItemEnumerator.ShouldSkipItem: Failed to retrieve SubscriptionId from itemProperty {0}.", item);
				return false;
			}
			if (this.ids.Contains(valueOrDefault))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "ActiveSubscriptionItemEnumerator.ShouldSkipItem: Replicated item found on ActiveSubscriptionEnumerator '{0}'.", valueOrDefault);
				return true;
			}
			this.ids.Add(valueOrDefault);
			return false;
		}

		private readonly uint expirationInHours;

		private HashSet<string> ids = new HashSet<string>();
	}
}
