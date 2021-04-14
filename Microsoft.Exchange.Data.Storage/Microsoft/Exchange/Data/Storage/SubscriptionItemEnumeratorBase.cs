using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SubscriptionItemEnumeratorBase : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		public SubscriptionItemEnumeratorBase(IFolder folder) : this(folder, SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		public SubscriptionItemEnumeratorBase(IFolder folder, Unlimited<uint> resultSize)
		{
			ArgumentValidator.ThrowIfNull("folder", folder);
			ArgumentValidator.ThrowIfInvalidValue<Unlimited<uint>>("resultSize", resultSize, (Unlimited<uint> x) => x.IsUnlimited || x.Value > 0U);
			this.folder = folder;
			this.maximumResultSize = resultSize;
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			using (IQueryResult query = this.folder.IItemQuery(ItemQueryType.None, null, this.GetSortByConstraint(), SubscriptionItemEnumeratorBase.PushNotificationSubscriptionItemProperties))
			{
				if (!query.SeekToCondition(SeekReference.OriginBeginning, SubscriptionItemEnumeratorBase.implicitQueryFilter, SeekToConditionFlags.AllowExtendedFilters))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<IFolder>((long)this.GetHashCode(), "SubscriptionItemEnumeratorBase.GetEnumerator: No Subscriptions found on the folder {0}.", this.folder);
					yield break;
				}
				int currentSize = 0;
				IStorePropertyBag[] messages = query.GetPropertyBags(50);
				while (messages.Length > 0)
				{
					foreach (IStorePropertyBag message in messages)
					{
						if (!this.maximumResultSize.IsUnlimited && (long)currentSize == (long)((ulong)this.maximumResultSize.Value))
						{
							ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<Unlimited<uint>>((long)this.GetHashCode(), "SubscriptionItemEnumeratorBase.GetEnumerator:Maximum Number of elements reached {0}.", this.maximumResultSize);
							yield break;
						}
						string itemClass = message.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
						if (string.IsNullOrEmpty(itemClass))
						{
							ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceWarning<IFolder>((long)this.GetHashCode(), "SubscriptionItemEnumeratorBase.GetEnumerator:An empty itemClass was retrieved from the folder {0}.", this.folder);
						}
						else
						{
							if (!itemClass.Equals("Exchange.PushNotification.Subscription", StringComparison.OrdinalIgnoreCase))
							{
								ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "SubscriptionItemEnumeratorBase.GetEnumerator: Found an item that does not match the Subscription itemClass.");
								yield break;
							}
							if (this.ShouldStopProcessingItems(message))
							{
								ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "SubscriptionItemEnumeratorBase.GetEnumerator: Processing items stopped by ShouldStopProcessingItems.");
								yield break;
							}
							if (this.ShouldSkipItem(message))
							{
								ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<IFolder>((long)this.GetHashCode(), "SubscriptionItemEnumeratorBase.GetEnumerator:Message requested to be skipped by ShouldSkipItem {0}.", this.folder);
							}
							else
							{
								currentSize++;
								yield return message;
							}
						}
					}
					messages = query.GetPropertyBags(50);
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		protected abstract SortBy[] GetSortByConstraint();

		protected abstract bool ShouldStopProcessingItems(IStorePropertyBag item);

		protected abstract bool ShouldSkipItem(IStorePropertyBag item);

		internal static readonly Unlimited<uint> EnumeratorDefaultMaximumSize = 50U;

		private static readonly QueryFilter implicitQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.ItemClass, "Exchange.PushNotification.Subscription");

		public static readonly PropertyDefinition[] PushNotificationSubscriptionItemProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			PushNotificationSubscriptionItemSchema.SubscriptionId,
			PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC,
			PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription
		};

		protected static readonly SortBy[] RefreshTimeUTCDescSortBy = new SortBy[]
		{
			new SortBy(PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC, SortOrder.Descending)
		};

		protected static readonly SortBy[] RefreshTimeUTCAscSortBy = new SortBy[]
		{
			new SortBy(PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC, SortOrder.Ascending)
		};

		private readonly IFolder folder;

		private readonly Unlimited<uint> maximumResultSize;
	}
}
