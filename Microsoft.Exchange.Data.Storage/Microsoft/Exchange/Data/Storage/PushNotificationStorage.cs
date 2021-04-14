using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PushNotificationStorage : DisposableObject, IPushNotificationStorage, IDisposable
	{
		private PushNotificationStorage(IFolder folder, string tenantId) : this(folder, tenantId, XSOFactory.Default)
		{
		}

		private PushNotificationStorage(IFolder folder, string tenantId, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("folder", xsoFactory);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			this.folder = folder;
			this.xsoFactory = xsoFactory;
			this.TenantId = tenantId;
		}

		public string TenantId { get; private set; }

		public static IPushNotificationStorage Create(IMailboxSession mailboxSession)
		{
			return PushNotificationStorage.Create(mailboxSession, XSOFactory.Default, OrganizationIdConvertor.Default);
		}

		public static IPushNotificationStorage Create(IMailboxSession mailboxSession, IXSOFactory xsoFactory)
		{
			return PushNotificationStorage.Create(mailboxSession, xsoFactory, OrganizationIdConvertor.Default);
		}

		public static void DeleteStorage(IMailboxSession mailboxSession)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			mailboxSession.DeleteDefaultFolder(DefaultFolderType.PushNotificationRoot, DeleteItemFlags.HardDelete);
		}

		public static IPushNotificationStorage Create(IMailboxSession mailboxSession, IXSOFactory xsoFactory, IOrganizationIdConvertor organizationIdConvertor)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			IPushNotificationStorage pushNotificationStorage = PushNotificationStorage.Find(mailboxSession, xsoFactory);
			if (pushNotificationStorage != null)
			{
				return pushNotificationStorage;
			}
			ArgumentValidator.ThrowIfNull("mailboxSession.MailboxOwner", mailboxSession.MailboxOwner);
			ArgumentValidator.ThrowIfNull("organizationIdConvertor", organizationIdConvertor);
			if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string>(0L, "PushNotificationStorage.Create: Creating a new Notification Subscription folder for user {0}.", (mailboxSession.MailboxOwner.ObjectId != null) ? mailboxSession.MailboxOwner.ObjectId.ToDNString() : string.Empty);
			}
			StoreObjectId folderId = mailboxSession.CreateDefaultFolder(DefaultFolderType.PushNotificationRoot);
			IFolder folder = xsoFactory.BindToFolder(mailboxSession, folderId);
			return new PushNotificationStorage(folder, PushNotificationStorage.GetTenantId(mailboxSession), xsoFactory);
		}

		public static IPushNotificationStorage Find(IMailboxSession mailboxSession)
		{
			return PushNotificationStorage.Find(mailboxSession, XSOFactory.Default);
		}

		public static IPushNotificationStorage Find(IMailboxSession mailboxSession, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.PushNotificationRoot);
			if (defaultFolderId != null)
			{
				return new PushNotificationStorage(xsoFactory.BindToFolder(mailboxSession, defaultFolderId), PushNotificationStorage.GetTenantId(mailboxSession));
			}
			return null;
		}

		public List<PushNotificationServerSubscription> GetActiveNotificationSubscriptions(IMailboxSession mailboxSession, uint expirationInHours = 72U)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "PushNotificationStorage.GetActiveNotificationSubscriptions: Requested active subscriptions under this folder.");
			return this.GetPushNotificationSubscriptions(mailboxSession, new ActiveSubscriptionItemEnumerator(this.folder, expirationInHours));
		}

		public List<StoreObjectId> GetExpiredNotificationSubscriptions(uint expirationInHours = 72U)
		{
			List<StoreObjectId> pushNotificationSubscriptionIds = this.GetPushNotificationSubscriptionIds(new ExpiredSubscriptionItemEnumerator(this.folder, expirationInHours, PushNotificationStorage.ExpiredSubscriptionItemThresholdPolicy + 1U));
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "PushNotificationStorage.GetExpiredNotificationSubscriptions: A total {0} expired subscription items found.", pushNotificationSubscriptionIds.Count);
			return pushNotificationSubscriptionIds;
		}

		public List<PushNotificationServerSubscription> GetNotificationSubscriptions(IMailboxSession mailboxSession)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "PushNotificationStorage.GetNotificationSubscriptions: Requested all subscription under this folder.");
			return this.GetPushNotificationSubscriptions(mailboxSession, new SubscriptionItemEnumerator(this.folder));
		}

		public IPushNotificationSubscriptionItem CreateOrUpdateSubscriptionItem(IMailboxSession mailboxSession, string subscriptionId, PushNotificationServerSubscription subscription)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNullOrEmpty("subscriptionId", subscriptionId);
			ArgumentValidator.ThrowIfNull("subscription", subscription);
			return PushNotificationSubscriptionItem.CreateOrUpdateSubscription(mailboxSession, this.xsoFactory, this.folder, subscriptionId, subscription);
		}

		public void DeleteExpiredSubscriptions(uint expirationInHours = 72U)
		{
			StoreObjectId[] array = this.GetExpiredNotificationSubscriptions(expirationInHours).ToArray();
			if ((long)array.Length > (long)((ulong)PushNotificationStorage.ExpiredSubscriptionItemThresholdPolicy))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "PushNotificationStorage.DeleteExpiredSubscriptions: Number of expired subscription items exceeded threshold {0}.", array.Length);
				this.folder.DeleteObjects(DeleteItemFlags.HardDelete, array);
			}
		}

		public void DeleteAllSubscriptions()
		{
			StoreObjectId[] array = this.GetPushNotificationSubscriptionIds(new SubscriptionItemEnumerator(this.folder)).ToArray();
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "PushNotificationStorage.DeleteAllSubscriptions: Number of subscription items to be deleted {0}.", array.Length);
			this.folder.DeleteObjects(DeleteItemFlags.HardDelete, array);
		}

		public void DeleteSubscription(StoreObjectId itemId)
		{
			ArgumentValidator.ThrowIfNull("itemId", itemId);
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "PushNotificationStorage.DeleteSubscription: Requested deletion of subscription {0}.", itemId);
			this.folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
			{
				itemId
			});
		}

		public void DeleteSubscription(string subscriptionId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("subscriptionId", subscriptionId);
			bool flag = false;
			foreach (IStorePropertyBag storePropertyBag in PushNotificationSubscriptionItem.GetSubscriptionById(this.folder, subscriptionId))
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

		internal static PushNotificationStorage GetNotificationFolderRoot(Folder folder)
		{
			return new PushNotificationStorage(folder, PushNotificationStorage.GetTenantId(folder.Session));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PushNotificationStorage>(this);
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

		private List<StoreObjectId> GetPushNotificationSubscriptionIds(SubscriptionItemEnumeratorBase enumerable)
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
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError((long)this.GetHashCode(), "PushNotificationStorage.GetPushNotificationSubscriptionIds: A subscription with an empty ItemSchema.Id value was returned by the Enumerator.");
			throw new CannotResolvePropertyException(ItemSchema.Id.Name);
		}

		private List<PushNotificationServerSubscription> GetPushNotificationSubscriptions(IMailboxSession mailboxSession, SubscriptionItemEnumeratorBase enumerable)
		{
			ArgumentValidator.ThrowIfNull("enumerable", enumerable);
			List<PushNotificationServerSubscription> list = new List<PushNotificationServerSubscription>();
			foreach (IStorePropertyBag propertyBag in enumerable)
			{
				string serializedNotificationSubscription = PushNotificationStorage.GetSerializedNotificationSubscription(mailboxSession, propertyBag, this.xsoFactory);
				list.Add(PushNotificationServerSubscription.FromJson(serializedNotificationSubscription));
			}
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "PushNotificationStorage.GetPushNotificationSubscriptions: A total {0} subscription items found.", list.Count);
			return list;
		}

		private static string GetTenantId(IStoreSession session)
		{
			string result = string.Empty;
			byte[] valueOrDefault = session.Mailbox.GetValueOrDefault<byte[]>(MailboxSchema.PersistableTenantPartitionHint, null);
			if (valueOrDefault != null && valueOrDefault.Length > 0)
			{
				TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(valueOrDefault);
				Guid externalDirectoryOrganizationId = tenantPartitionHint.GetExternalDirectoryOrganizationId();
				result = (Guid.Empty.Equals(externalDirectoryOrganizationId) ? string.Empty : externalDirectoryOrganizationId.ToString());
			}
			return result;
		}

		public static string GetSerializedNotificationSubscription(IMailboxSession mailboxSession, IStorePropertyBag propertyBag, IXSOFactory xsoFactory)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription, null);
			if (string.IsNullOrWhiteSpace(valueOrDefault))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError(0L, "PushNotificationStorage.GetSerializedNotificationSubscription: A subscription with an empty serialized value was returned by the Enumerator.");
				throw new CannotResolvePropertyException(PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription.Name);
			}
			if (valueOrDefault.Length < 255)
			{
				return valueOrDefault;
			}
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug(0L, "PushNotificationStorage.GetSerializedNotificationSubscription: We need to bind to the item in order to obtain the full serialized notification subscription.");
			VersionedId valueOrDefault2 = propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			string result;
			using (IPushNotificationSubscriptionItem pushNotificationSubscriptionItem = xsoFactory.BindToPushNotificationSubscriptionItem(mailboxSession, valueOrDefault2, new PropertyDefinition[]
			{
				PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription
			}))
			{
				string serializedNotificationSubscription = pushNotificationSubscriptionItem.SerializedNotificationSubscription;
				if (string.IsNullOrWhiteSpace(serializedNotificationSubscription))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<VersionedId, string>(0L, "PushNotificationStorage.GetFullSerializedNotificationSubscription: Unable to obtain the full SerializedNotificationSubscription from {0}, partial value: {1}", valueOrDefault2, valueOrDefault);
					throw new CannotResolvePropertyException(PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription.Name);
				}
				result = serializedNotificationSubscription;
			}
			return result;
		}

		public const uint DefaultSubscriptionExpirationInHours = 72U;

		public static readonly uint ExpiredSubscriptionItemThresholdPolicy = (!SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize.IsUnlimited) ? (SubscriptionItemEnumeratorBase.EnumeratorDefaultMaximumSize.Value * 8U / 10U) : 80U;

		private IFolder folder;

		private IXSOFactory xsoFactory;
	}
}
