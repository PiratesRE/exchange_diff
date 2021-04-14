using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class OutlookServiceSubscriptionItemEnumeratorBase : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		public OutlookServiceSubscriptionItemEnumeratorBase(IFolder folder) : this(folder, null, OutlookServiceSubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		public OutlookServiceSubscriptionItemEnumeratorBase(IFolder folder, string appId) : this(folder, appId, OutlookServiceSubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize)
		{
		}

		public OutlookServiceSubscriptionItemEnumeratorBase(IFolder folder, string appId, Unlimited<uint> resultSize)
		{
			ArgumentValidator.ThrowIfNull("folder", folder);
			ArgumentValidator.ThrowIfInvalidValue<Unlimited<uint>>("resultSize", resultSize, (Unlimited<uint> x) => x.IsUnlimited || x.Value > 0U);
			this.folder = folder;
			this.appId = appId;
			this.maximumResultSize = resultSize;
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			using (IQueryResult query = this.folder.IItemQuery(ItemQueryType.None, null, this.GetSortByConstraint(), OutlookServiceSubscriptionItemEnumeratorBase.OutlookServiceSubscriptionItemProperties))
			{
				if (!query.SeekToCondition(SeekReference.OriginBeginning, OutlookServiceSubscriptionItemEnumeratorBase.implicitQueryFilter, SeekToConditionFlags.AllowExtendedFilters))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<IFolder>((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.GetEnumerator: No Subscriptions found on the folder {0}.", this.folder);
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
							ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<Unlimited<uint>>((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.GetEnumerator:Maximum Number of elements reached {0}.", this.maximumResultSize);
							yield break;
						}
						string itemClass = message.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
						if (string.IsNullOrEmpty(itemClass))
						{
							ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceWarning<IFolder>((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.GetEnumerator:An empty itemClass was retrieved from the folder {0}.", this.folder);
						}
						else
						{
							if (!itemClass.Equals("OutlookService.Notification.Subscription", StringComparison.OrdinalIgnoreCase))
							{
								ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.GetEnumerator: Found an item that does not match the Subscription itemClass.");
								yield break;
							}
							if (!this.CorrectAppId(message))
							{
								ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.GetEnumerator:  Found an item that does not match the AppId being Enumerated.");
								yield break;
							}
							if (this.ShouldStopProcessingItems(message))
							{
								ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.GetEnumerator: Processing items stopped by ShouldStopProcessingItems.");
								yield break;
							}
							if (this.ShouldSkipItem(message))
							{
								ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<IFolder>((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.GetEnumerator:Message requested to be skipped by ShouldSkipItem {0}.", this.folder);
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

		protected bool CorrectAppId(IStorePropertyBag item)
		{
			if (this.appId == null)
			{
				return true;
			}
			string valueOrDefault = item.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.AppId, null);
			if (valueOrDefault == null)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<IStorePropertyBag>((long)this.GetHashCode(), "OutlookServiceSubscriptionItemEnumeratorBase.CorrectAppId: Failed to retrieve AppId from itemProperty {0}.", item);
				return false;
			}
			return valueOrDefault == this.appId;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		protected abstract SortBy[] GetSortByConstraint();

		protected abstract bool ShouldStopProcessingItems(IStorePropertyBag item);

		protected abstract bool ShouldSkipItem(IStorePropertyBag item);

		internal static readonly Unlimited<uint> EnumeratorDefaultMaximumSize = 50U;

		private static readonly QueryFilter implicitQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.ItemClass, "OutlookService.Notification.Subscription");

		public static readonly PropertyDefinition[] OutlookServiceSubscriptionItemProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			OutlookServiceSubscriptionItemSchema.SubscriptionId,
			OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC,
			OutlookServiceSubscriptionItemSchema.AppId,
			OutlookServiceSubscriptionItemSchema.DeviceNotificationId,
			OutlookServiceSubscriptionItemSchema.ExpirationTime,
			OutlookServiceSubscriptionItemSchema.LockScreen
		};

		protected static readonly SortBy[] RefreshTimeUTCDescSortBy = new SortBy[]
		{
			new SortBy(OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC, SortOrder.Descending)
		};

		protected static readonly SortBy[] RefreshTimeUTCAscSortBy = new SortBy[]
		{
			new SortBy(OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC, SortOrder.Ascending)
		};

		protected static readonly SortBy[] ExpirationTimeDescSortBy = new SortBy[]
		{
			new SortBy(OutlookServiceSubscriptionItemSchema.ExpirationTime, SortOrder.Descending)
		};

		protected static readonly SortBy[] ExpirationTimeAscSortBy = new SortBy[]
		{
			new SortBy(OutlookServiceSubscriptionItemSchema.ExpirationTime, SortOrder.Ascending)
		};

		private readonly IFolder folder;

		private readonly string appId;

		private readonly Unlimited<uint> maximumResultSize;
	}
}
