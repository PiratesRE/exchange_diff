using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PushNotificationSubscriptionItem : Item, IPushNotificationSubscriptionItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal PushNotificationSubscriptionItem(ICoreItem coreItem) : base(coreItem, false)
		{
			if (base.IsNew)
			{
				this.Initialize();
			}
		}

		public string SubscriptionId
		{
			get
			{
				this.CheckDisposed("Type::get");
				return base.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SubscriptionId, null);
			}
			set
			{
				this.CheckDisposed("Type::set");
				this[PushNotificationSubscriptionItemSchema.SubscriptionId] = value;
			}
		}

		public ExDateTime LastUpdateTimeUTC
		{
			get
			{
				this.CheckDisposed("Type::get");
				return base.GetValueOrDefault<ExDateTime>(PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC, ExDateTime.UtcNow);
			}
			set
			{
				this.CheckDisposed("Type::set");
				this[PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC] = value;
			}
		}

		public string SerializedNotificationSubscription
		{
			get
			{
				this.CheckDisposed("Type::get");
				return base.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription, null);
			}
			set
			{
				this.CheckDisposed("Type::set");
				this[PushNotificationSubscriptionItemSchema.SerializedNotificationSubscription] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return PushNotificationSubscriptionItemSchema.Instance;
			}
		}

		public static IPushNotificationSubscriptionItem CreateOrUpdateSubscription(IMailboxSession session, IXSOFactory xsoFactory, IFolder folder, string subscriptionId, PushNotificationServerSubscription subscription)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			Util.ThrowOnNullArgument(folder, "folder");
			Util.ThrowOnNullOrEmptyArgument(subscriptionId, "subscriptionId");
			Util.ThrowOnNullArgument(subscription, "subscription");
			ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string, IExchangePrincipal>((long)subscription.GetHashCode(), "PushNotificationSubscriptionItem.CreateOrUpdateSubscription: Searching for Subscription {0} on Mailbox {1}.", subscriptionId, session.MailboxOwner);
			IStorePropertyBag[] array = PushNotificationSubscriptionItem.GetSubscriptionById(folder, subscriptionId).ToArray<IStorePropertyBag>();
			IPushNotificationSubscriptionItem pushNotificationSubscriptionItem = null;
			try
			{
				if (array.Length >= 1)
				{
					if (array.Length > 1)
					{
						ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceWarning<string, Guid>(0L, "PushNotificationSubscriptionItem.CreateOrUpdateSubscription: AmbiguousSubscription for subscription {0} and user {1}", subscriptionId, session.MailboxGuid);
					}
					IStorePropertyBag storePropertyBag = array[0];
					VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
					if (valueOrDefault == null)
					{
						ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<string>((long)storePropertyBag.GetHashCode(), "PushNotificationSubscriptionItem.CreateOrUpdateSubscription: Cannot resolve the ItemSchema.Id property from the Enumerable.", subscriptionId);
						throw new CannotResolvePropertyException(ItemSchema.Id.Name);
					}
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<VersionedId>((long)storePropertyBag.GetHashCode(), "PushNotificationSubscriptionItem.CreateOrUpdateSubscription: Found one existing subscription with ItemSchema.Id = {0}.", valueOrDefault);
					pushNotificationSubscriptionItem = xsoFactory.BindToPushNotificationSubscriptionItem(session, valueOrDefault, null);
					pushNotificationSubscriptionItem.LastUpdateTimeUTC = ExDateTime.UtcNow;
					subscription.LastSubscriptionUpdate = (DateTime)pushNotificationSubscriptionItem.LastUpdateTimeUTC;
					pushNotificationSubscriptionItem.SerializedNotificationSubscription = subscription.ToJson();
					ConflictResolutionResult conflictResolutionResult = pushNotificationSubscriptionItem.Save(SaveMode.ResolveConflicts);
					if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
					{
						ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<string>((long)storePropertyBag.GetHashCode(), "PushNotificationSubscriptionItem.CreateOrUpdateSubscription: Save failed due to conflicts for subscription {0}.", subscriptionId);
						throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(subscriptionId));
					}
					pushNotificationSubscriptionItem.Load(SubscriptionItemEnumeratorBase.PushNotificationSubscriptionItemProperties);
				}
				else
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug((long)subscription.GetHashCode(), "PushNotificationSubscriptionItem.CreateOrUpdateSubscription: Cannot resolve given subscription, about to create a new SubscriptionItem.");
					pushNotificationSubscriptionItem = PushNotificationSubscriptionItem.Create(session, xsoFactory, folder.StoreObjectId, subscriptionId, subscription);
				}
			}
			catch
			{
				if (pushNotificationSubscriptionItem != null)
				{
					pushNotificationSubscriptionItem.Dispose();
				}
				throw;
			}
			return pushNotificationSubscriptionItem;
		}

		public static string GenerateSubscriptionId(string protocol, string deviceId, string deviceType)
		{
			return string.Format("{0}-{1}-{2}", protocol, deviceType, deviceId);
		}

		public static IEnumerable<IStorePropertyBag> GetSubscriptionById(IFolder folder, string subscriptionId)
		{
			Util.ThrowOnNullArgument(folder, "folder");
			Util.ThrowOnNullOrEmptyArgument(subscriptionId, "subscriptionId");
			return new SubscriptionItemEnumerator(folder).Where(delegate(IStorePropertyBag x)
			{
				string valueOrDefault = x.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SubscriptionId, null);
				return !string.IsNullOrEmpty(valueOrDefault) && valueOrDefault.Equals(subscriptionId);
			});
		}

		private static IPushNotificationSubscriptionItem Create(IMailboxSession session, IXSOFactory xsoFactory, StoreId locationFolderId, string subscriptionId, PushNotificationServerSubscription subscription)
		{
			IPushNotificationSubscriptionItem pushNotificationSubscriptionItem = null;
			try
			{
				pushNotificationSubscriptionItem = xsoFactory.CreatePushNotificationSubscriptionItem(session, locationFolderId);
				subscription.LastSubscriptionUpdate = (DateTime)pushNotificationSubscriptionItem.LastUpdateTimeUTC;
				pushNotificationSubscriptionItem.SubscriptionId = subscriptionId;
				pushNotificationSubscriptionItem.SerializedNotificationSubscription = subscription.ToJson();
				if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string, ExDateTime, string>((long)subscription.GetHashCode(), "PushNotificationSubscriptionItem.Create: Created SubscriptionItem on store, Id:{0}, RefTm:{1}, Json:{2}", pushNotificationSubscriptionItem.SubscriptionId, pushNotificationSubscriptionItem.LastUpdateTimeUTC, pushNotificationSubscriptionItem.SerializedNotificationSubscription);
				}
				ConflictResolutionResult conflictResolutionResult = pushNotificationSubscriptionItem.Save(SaveMode.FailOnAnyConflict);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceError<string>((long)pushNotificationSubscriptionItem.GetHashCode(), "PushNotificationSubscriptionItem.Create: Save failed due to conflicts for subscription {0}.", subscriptionId);
					throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(pushNotificationSubscriptionItem.SubscriptionId));
				}
				pushNotificationSubscriptionItem.Load(SubscriptionItemEnumeratorBase.PushNotificationSubscriptionItemProperties);
			}
			catch
			{
				if (pushNotificationSubscriptionItem != null)
				{
					pushNotificationSubscriptionItem.Dispose();
				}
				throw;
			}
			return pushNotificationSubscriptionItem;
		}

		private void Initialize()
		{
			this[InternalSchema.ItemClass] = "Exchange.PushNotification.Subscription";
			this[PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC] = ExDateTime.UtcNow;
			if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "PushNotificationSubscriptionItem.Initialize: Initialized new SubscriptionItem, RefTm:{1}", this[PushNotificationSubscriptionItemSchema.LastUpdateTimeUTC].ToString());
			}
		}
	}
}
