using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookServiceSubscriptionStorage : DisposableObject, IOutlookServiceSubscriptionStorage, IDisposable
	{
		internal OutlookServiceSubscriptionStorage(IMailboxSession mailboxSession, IFolder folder, string tenantId) : this(mailboxSession, folder, tenantId, XSOFactory.Default)
		{
		}

		internal OutlookServiceSubscriptionStorage(IMailboxSession mailboxSession, IFolder folder, string tenantId, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("folder", folder);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("tenantId", tenantId);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			this.mailboxSession = mailboxSession;
			this.folder = folder;
			this.xsoFactory = xsoFactory;
			this.TenantId = tenantId;
		}

		public string TenantId { get; private set; }

		public static IOutlookServiceSubscriptionStorage Create(IMailboxSession mailboxSession, IFolder folder, string tenantId)
		{
			return new OutlookServiceSubscriptionStorage(mailboxSession, folder, tenantId);
		}

		public List<OutlookServiceNotificationSubscription> GetActiveNotificationSubscriptions(string appId, uint deactivationInHours = 72U)
		{
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetActiveNotificationSubscriptions: Requested active subscriptions under this folder.");
			return this.GetOutlookServiceSubscriptions(new ActiveOutlookServiceSubscriptionItemEnumerator(this.folder, appId, deactivationInHours));
		}

		public List<OutlookServiceNotificationSubscription> GetActiveNotificationSubscriptionsForContext(string notificationContext)
		{
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetActiveNotificationSubscriptionsForContext: Requested active subscriptions under this folder for a context.");
			List<OutlookServiceNotificationSubscription> list = new List<OutlookServiceNotificationSubscription>();
			foreach (OutlookServiceNotificationSubscription outlookServiceNotificationSubscription in this.GetActiveNotificationSubscriptions(null, 72U))
			{
				if (!(outlookServiceNotificationSubscription.AppId == OutlookServiceNotificationSubscription.AppId_HxMail) || !(outlookServiceNotificationSubscription.SubscriptionId != notificationContext))
				{
					list.Add(outlookServiceNotificationSubscription);
				}
			}
			return list;
		}

		public List<string> GetDeactivatedNotificationSubscriptions(string appId, uint deactivationInHours = 72U)
		{
			List<StoreObjectId> outlookServiceSubscriptionIds = this.GetOutlookServiceSubscriptionIds(new DeactivatedOutlookServiceSubscriptionItemEnumerator(this.folder, appId, deactivationInHours));
			List<string> list = new List<string>();
			foreach (StoreObjectId storeId in outlookServiceSubscriptionIds)
			{
				using (IOutlookServiceSubscriptionItem outlookServiceSubscriptionItem = this.xsoFactory.BindToOutlookServiceSubscriptionItem(this.mailboxSession, storeId, null))
				{
					new OutlookServiceNotificationSubscription(outlookServiceSubscriptionItem);
					list.Add(outlookServiceSubscriptionItem.SubscriptionId);
				}
			}
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetDeactivatedNotificationSubscriptions: A total {0} deactivated subscription items found.", list.Count);
			return list;
		}

		public List<string> GetExpiredNotificationSubscriptions(string appId)
		{
			List<StoreObjectId> outlookServiceSubscriptionIds = this.GetOutlookServiceSubscriptionIds(new ExpiredOutlookServiceSubscriptionItemEnumerator(this.folder, appId, OutlookServiceSubscriptionStorage.ExpiredSubscriptionItemThresholdPolicy + 1U));
			List<string> list = new List<string>();
			foreach (StoreObjectId storeId in outlookServiceSubscriptionIds)
			{
				using (IOutlookServiceSubscriptionItem outlookServiceSubscriptionItem = this.xsoFactory.BindToOutlookServiceSubscriptionItem(this.mailboxSession, storeId, null))
				{
					new OutlookServiceNotificationSubscription(outlookServiceSubscriptionItem);
					list.Add(outlookServiceSubscriptionItem.SubscriptionId);
				}
			}
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetExpiredNotificationSubscriptions: A total {0} expired subscription items found.", list.Count);
			return list;
		}

		public List<StoreObjectId> GetExpiredNotificationStoreIds(string appId)
		{
			List<StoreObjectId> outlookServiceSubscriptionIds = this.GetOutlookServiceSubscriptionIds(new ExpiredOutlookServiceSubscriptionItemEnumerator(this.folder, appId, OutlookServiceSubscriptionStorage.ExpiredSubscriptionItemThresholdPolicy + 1U));
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetExpiredNotificationSubscriptions: A total {0} expired subscription items found.", outlookServiceSubscriptionIds.Count);
			return outlookServiceSubscriptionIds;
		}

		public List<OutlookServiceNotificationSubscription> GetNotificationSubscriptions(string appId)
		{
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetNotificationSubscriptions: Requested all subscription under this folder.");
			return this.GetOutlookServiceSubscriptions(new OutlookServiceSubscriptionItemEnumerator(this.folder, appId));
		}

		public OutlookServiceNotificationSubscription CreateOrUpdateSubscriptionItem(OutlookServiceNotificationSubscription subscription)
		{
			ArgumentValidator.ThrowIfNull("subscription", subscription);
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string, IExchangePrincipal>(0L, "OutlookServiceSubscriptionItem.CreateOrUpdateSubscription: Searching for Subscription {0} on Mailbox {1}.", subscription.SubscriptionId, this.mailboxSession.MailboxOwner);
			IStorePropertyBag[] array = OutlookServiceSubscriptionStorage.GetSubscriptionById(this.folder, subscription.SubscriptionId).ToArray<IStorePropertyBag>();
			IOutlookServiceSubscriptionItem outlookServiceSubscriptionItem = null;
			OutlookServiceNotificationSubscription result = null;
			try
			{
				if (array.Length >= 1)
				{
					if (array.Length > 1)
					{
						ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceWarning<string, Guid>(0L, "OutlookServiceSubscriptionItem.CreateOrUpdateSubscription: AmbiguousSubscription for subscription {0} and user {1}", subscription.SubscriptionId, this.mailboxSession.MailboxGuid);
					}
					IStorePropertyBag storePropertyBag = array[0];
					VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
					if (valueOrDefault == null)
					{
						ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<string>((long)storePropertyBag.GetHashCode(), "OutlookServiceSubscriptionItem.CreateOrUpdateSubscription: Cannot resolve the ItemSchema.Id property from the Enumerable.", subscription.SubscriptionId);
						throw new CannotResolvePropertyException(ItemSchema.Id.Name);
					}
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<VersionedId>((long)storePropertyBag.GetHashCode(), "OutlookServiceSubscriptionItem.CreateOrUpdateSubscription: Found one existing subscription with ItemSchema.Id = {0}.", valueOrDefault);
					outlookServiceSubscriptionItem = this.xsoFactory.BindToOutlookServiceSubscriptionItem(this.mailboxSession, valueOrDefault, null);
					outlookServiceSubscriptionItem.LastUpdateTimeUTC = ExDateTime.UtcNow;
					outlookServiceSubscriptionItem.AppId = subscription.AppId;
					outlookServiceSubscriptionItem.DeviceNotificationId = subscription.DeviceNotificationId;
					if (subscription.ExpirationTime != null)
					{
						outlookServiceSubscriptionItem.ExpirationTime = subscription.ExpirationTime.Value;
					}
					outlookServiceSubscriptionItem.LockScreen = subscription.LockScreen;
					ConflictResolutionResult conflictResolutionResult = outlookServiceSubscriptionItem.Save(SaveMode.ResolveConflicts);
					if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
					{
						ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<string>((long)storePropertyBag.GetHashCode(), "OutlookServiceSubscriptionItem.CreateOrUpdateSubscription: Save failed due to conflicts for subscription {0}.", subscription.SubscriptionId);
						throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(subscription.SubscriptionId));
					}
					outlookServiceSubscriptionItem.Load(OutlookServiceSubscriptionItemEnumeratorBase.OutlookServiceSubscriptionItemProperties);
				}
				else
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug(0L, "OutlookServiceSubscriptionItem.CreateOrUpdateSubscription: Cannot resolve given subscription, about to create a new SubscriptionItem.");
					outlookServiceSubscriptionItem = this.CreateSubscriptionItem(subscription);
				}
				result = new OutlookServiceNotificationSubscription(outlookServiceSubscriptionItem);
			}
			finally
			{
				if (outlookServiceSubscriptionItem != null)
				{
					outlookServiceSubscriptionItem.Dispose();
				}
			}
			return result;
		}

		private IOutlookServiceSubscriptionItem CreateSubscriptionItem(OutlookServiceNotificationSubscription subscription)
		{
			IOutlookServiceSubscriptionItem outlookServiceSubscriptionItem = null;
			try
			{
				outlookServiceSubscriptionItem = this.xsoFactory.CreateOutlookServiceSubscriptionItem(this.mailboxSession, this.folder.StoreObjectId);
				outlookServiceSubscriptionItem.SubscriptionId = subscription.SubscriptionId;
				outlookServiceSubscriptionItem.AppId = subscription.AppId;
				outlookServiceSubscriptionItem.DeviceNotificationId = subscription.DeviceNotificationId;
				if (subscription.ExpirationTime != null)
				{
					outlookServiceSubscriptionItem.ExpirationTime = subscription.ExpirationTime.Value;
				}
				outlookServiceSubscriptionItem.LockScreen = subscription.LockScreen;
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)outlookServiceSubscriptionItem.GetHashCode(), "OutlookServiceSubscriptionItem.Create: Created SubscriptionItem on store, Id:{0}, RefTm:{1}, AppId:{2}, DevNotifId:{3}, Expire:{4}, Lock:{5}", new object[]
					{
						outlookServiceSubscriptionItem.SubscriptionId,
						outlookServiceSubscriptionItem.LastUpdateTimeUTC,
						outlookServiceSubscriptionItem.AppId,
						outlookServiceSubscriptionItem.DeviceNotificationId,
						outlookServiceSubscriptionItem.ExpirationTime.ToString(),
						outlookServiceSubscriptionItem.LockScreen
					});
				}
				ConflictResolutionResult conflictResolutionResult = outlookServiceSubscriptionItem.Save(SaveMode.FailOnAnyConflict);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<string>((long)outlookServiceSubscriptionItem.GetHashCode(), "OutlookServiceSubscriptionItem.Create: Save failed due to conflicts for subscription {0}.", subscription.SubscriptionId);
					throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(outlookServiceSubscriptionItem.SubscriptionId));
				}
				outlookServiceSubscriptionItem.Load(OutlookServiceSubscriptionItemEnumeratorBase.OutlookServiceSubscriptionItemProperties);
			}
			catch
			{
				if (outlookServiceSubscriptionItem != null)
				{
					outlookServiceSubscriptionItem.Dispose();
				}
				throw;
			}
			return outlookServiceSubscriptionItem;
		}

		public static IEnumerable<IStorePropertyBag> GetSubscriptionById(IFolder folder, string subscriptionId)
		{
			Util.ThrowOnNullArgument(folder, "folder");
			Util.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			return new OutlookServiceSubscriptionItemEnumerator(folder).Where(delegate(IStorePropertyBag x)
			{
				string valueOrDefault = x.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.SubscriptionId, null);
				return !string.IsNullOrEmpty(valueOrDefault) && valueOrDefault.Equals(subscriptionId);
			});
		}

		public void DeleteExpiredSubscriptions(string appId)
		{
			StoreObjectId[] array = this.GetExpiredNotificationStoreIds(appId).ToArray();
			if ((long)array.Length > (long)((ulong)OutlookServiceSubscriptionStorage.ExpiredSubscriptionItemThresholdPolicy))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.DeleteExpiredSubscriptions: Number of expired subscription items exceeded threshold {0}.", array.Length);
				this.folder.DeleteObjects(DeleteItemFlags.HardDelete, array);
			}
		}

		public void DeleteAllSubscriptions(string appId)
		{
			StoreObjectId[] array = this.GetOutlookServiceSubscriptionIds(new OutlookServiceSubscriptionItemEnumerator(this.folder, appId)).ToArray();
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.DeleteAllSubscriptions: Number of subscription items to be deleted {0}.", array.Length);
			this.folder.DeleteObjects(DeleteItemFlags.HardDelete, array);
		}

		public void DeleteSubscription(StoreObjectId itemId)
		{
			ArgumentValidator.ThrowIfNull("itemId", itemId);
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.DeleteSubscription: Requested deletion of subscription {0}.", itemId);
			this.folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
			{
				itemId
			});
		}

		public void DeleteSubscription(string subscriptionId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("subscriptionId", subscriptionId);
			bool flag = false;
			foreach (IStorePropertyBag storePropertyBag in OutlookServiceSubscriptionStorage.GetSubscriptionById(this.folder, subscriptionId))
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				if (valueOrDefault == null)
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError((long)this.GetHashCode(), "NotificationStorage.DeleteSubscription: A subscription with an empty ItemSchema.Id value was returned by the Enumerator.");
					throw new CannotResolvePropertyException(ItemSchema.Id.Name);
				}
				this.DeleteSubscription(valueOrDefault.ObjectId);
				flag = true;
			}
			if (!flag)
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "NotificationStorage.DeleteSubscription: Did not find the subscription to delete.");
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<OutlookServiceSubscriptionStorage>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
			base.InternalDispose(disposing);
		}

		private List<StoreObjectId> GetOutlookServiceSubscriptionIds(OutlookServiceSubscriptionItemEnumeratorBase enumerable)
		{
			List<StoreObjectId> list = new List<StoreObjectId>();
			foreach (IStorePropertyBag propertyBag in enumerable)
			{
				list.Add(this.GetVersionedId(propertyBag).ObjectId);
			}
			return list;
		}

		private VersionedId GetVersionedId(IStorePropertyBag propertyBag)
		{
			VersionedId valueOrDefault = propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			if (valueOrDefault != null)
			{
				return valueOrDefault;
			}
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetOutlookServiceSubscriptionIds: A subscription with an empty ItemSchema.Id value was returned by the Enumerator.");
			throw new CannotResolvePropertyException(ItemSchema.Id.Name);
		}

		private List<OutlookServiceNotificationSubscription> GetOutlookServiceSubscriptions(OutlookServiceSubscriptionItemEnumeratorBase enumerable)
		{
			ArgumentValidator.ThrowIfNull("enumerable", enumerable);
			List<OutlookServiceNotificationSubscription> list = new List<OutlookServiceNotificationSubscription>();
			foreach (IStorePropertyBag storePropertyBag in enumerable)
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				using (IOutlookServiceSubscriptionItem outlookServiceSubscriptionItem = this.xsoFactory.BindToOutlookServiceSubscriptionItem(this.mailboxSession, valueOrDefault, null))
				{
					OutlookServiceNotificationSubscription item = new OutlookServiceNotificationSubscription(outlookServiceSubscriptionItem);
					list.Add(item);
				}
			}
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "OutlookServiceSubscriptionStorage.GetOutlookServiceSubscriptions: A total {0} subscription items found.", list.Count);
			return list;
		}

		public static readonly uint ExpiredSubscriptionItemThresholdPolicy = (!OutlookServiceSubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize.IsUnlimited) ? (OutlookServiceSubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize.Value * 8U / 10U) : 80U;

		private IFolder folder;

		private IMailboxSession mailboxSession;

		private IXSOFactory xsoFactory;
	}
}
