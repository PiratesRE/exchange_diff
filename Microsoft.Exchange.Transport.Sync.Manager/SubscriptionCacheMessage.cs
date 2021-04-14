using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SubscriptionCacheMessage : DisposeTrackableBase
	{
		internal SubscriptionCacheMessage(GlobalSyncLogSession syncLogSession, MessageItem message, Guid mailboxGuid, ExDateTime subscriptionListTimestamp) : this(syncLogSession, message, mailboxGuid, true)
		{
			message.ClassName = "IPM.Aggregation.Cache.Subscriptions";
			message[MessageItemSchema.SharingInstanceGuid] = mailboxGuid;
			this.subscriptionListTimestamp = subscriptionListTimestamp;
		}

		private SubscriptionCacheMessage(GlobalSyncLogSession syncLogSession, MessageItem message, Guid mailboxGuid, bool newlyCreated)
		{
			if (message.Id != null)
			{
				this.id = message.Id.ObjectId;
			}
			else
			{
				this.id = null;
			}
			this.syncLogSession = syncLogSession;
			this.message = message;
			this.mailboxGuid = mailboxGuid;
			this.subscriptionCacheEntries = new LinkedList<SubscriptionCacheEntry>();
			this.newlyCreated = newlyCreated;
		}

		internal bool IsNew
		{
			get
			{
				base.CheckDisposed();
				return this.newlyCreated;
			}
		}

		internal StoreObjectId Id
		{
			get
			{
				base.CheckDisposed();
				return this.id;
			}
		}

		internal int SubscriptionCount
		{
			get
			{
				base.CheckDisposed();
				this.CheckSubscriptionsLoaded();
				return this.subscriptionCacheEntries.Count;
			}
		}

		internal ExDateTime SubscriptionListTimestamp
		{
			get
			{
				base.CheckDisposed();
				this.CheckSubscriptionsLoaded();
				return this.subscriptionListTimestamp;
			}
			set
			{
				base.CheckDisposed();
				this.CheckSubscriptionsLoaded();
				this.subscriptionListTimestamp = value;
			}
		}

		internal IEnumerable<SubscriptionCacheEntry> Subscriptions
		{
			get
			{
				base.CheckDisposed();
				this.CheckSubscriptionsLoaded();
				return this.subscriptionCacheEntries;
			}
		}

		internal static SubscriptionCacheMessage Bind(GlobalSyncLogSession syncLogSession, MailboxSession systemMailboxSession, StoreObjectId cacheMessageId)
		{
			SyncUtilities.ThrowIfArgumentNull("systemMailboxSession", systemMailboxSession);
			SyncUtilities.ThrowIfArgumentNull("cacheMessageId", cacheMessageId);
			MessageItem messageItem = MessageItem.Bind(systemMailboxSession, cacheMessageId, SubscriptionCacheMessage.BindProperties);
			Guid guid = (Guid)messageItem[MessageItemSchema.SharingInstanceGuid];
			return new SubscriptionCacheMessage(syncLogSession, messageItem, guid, false);
		}

		internal static SubscriptionCacheMessage Bind(GlobalSyncLogSession syncLogSession, MailboxSession systemMailboxSession, Guid mailboxGuid, StoreObjectId cacheFolderId)
		{
			SyncUtilities.ThrowIfArgumentNull("systemMailboxSession", systemMailboxSession);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("cacheFolderId", cacheFolderId);
			List<StoreId> list = new List<StoreId>();
			using (Folder folder = Folder.Bind(systemMailboxSession, cacheFolderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, SubscriptionCacheMessage.FindCacheMessageSortBySharingInstanceGuid, SubscriptionCacheMessage.FindCacheMessageProperties))
				{
					ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, MessageItemSchema.SharingInstanceGuid, mailboxGuid);
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
					{
						object[][] rows = queryResult.GetRows(5);
						while (rows.Length != 0 && mailboxGuid.Equals((Guid)rows[0][0]))
						{
							int num = 0;
							while (num < rows.Length && mailboxGuid.Equals((Guid)rows[num][0]))
							{
								list.Add((StoreId)rows[num][1]);
								num++;
							}
							rows = queryResult.GetRows(5);
						}
					}
				}
			}
			if (list.Count > 1)
			{
				syncLogSession.LogError((TSLID)206UL, SubscriptionCacheMessage.Tracer, Guid.Empty, mailboxGuid, "{0} duplicate cache messages found for mailbox {1}.", new object[]
				{
					list.Count,
					mailboxGuid
				});
				foreach (StoreId storeId in list)
				{
					syncLogSession.LogVerbose((TSLID)207UL, SubscriptionCacheMessage.Tracer, Guid.Empty, mailboxGuid, "Deleting duplicate cache message with store ID {0} for mailbox {1}.", new object[]
					{
						storeId.ToString(),
						mailboxGuid
					});
					systemMailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						storeId
					});
				}
				DataAccessLayer.ReportWatson("Duplicate cache messages found.", new CacheTransientException(systemMailboxSession.MdbGuid, mailboxGuid, new InvalidOperationException("Duplicate cache messages found.")));
				return null;
			}
			if (list.Count == 1)
			{
				MessageItem messageItem = MessageItem.Bind(systemMailboxSession, list[0], SubscriptionCacheMessage.BindProperties);
				return new SubscriptionCacheMessage(syncLogSession, messageItem, mailboxGuid, false);
			}
			return null;
		}

		internal SubscriptionCacheEntry FindSubscriptionByMessageId(StoreObjectId subscriptionMessageId)
		{
			base.CheckDisposed();
			this.CheckSubscriptionsLoaded();
			foreach (SubscriptionCacheEntry subscriptionCacheEntry in this.subscriptionCacheEntries)
			{
				if (object.Equals(subscriptionCacheEntry.SubscriptionMessageId, subscriptionMessageId))
				{
					return subscriptionCacheEntry;
				}
			}
			return null;
		}

		internal SubscriptionCacheEntry FindSubscriptionBySubscriptionGuid(Guid subscriptionGuid)
		{
			base.CheckDisposed();
			this.CheckSubscriptionsLoaded();
			foreach (SubscriptionCacheEntry subscriptionCacheEntry in this.subscriptionCacheEntries)
			{
				if (object.Equals(subscriptionCacheEntry.SubscriptionGuid, subscriptionGuid))
				{
					return subscriptionCacheEntry;
				}
			}
			return null;
		}

		internal void Save()
		{
			base.CheckDisposed();
			this.OpenMessageAsReadWrite();
			if (this.loadedSubscriptions || this.newlyCreated)
			{
				using (Stream stream = this.OpenSubscriptionsSerializationStream())
				{
					BinaryWriter writer = new BinaryWriter(stream);
					this.Serialize(writer);
				}
			}
			this.SaveMessage();
			this.syncLogSession.LogDebugging((TSLID)153UL, SubscriptionCacheMessage.Tracer, Guid.Empty, this.mailboxGuid, "Saved Cache Message with Id:{0}, IsNew:{1}, Last Repair Time:{2}.", new object[]
			{
				this.id,
				this.IsNew,
				this.subscriptionListTimestamp
			});
		}

		internal void Load()
		{
			base.CheckDisposed();
			this.message.Load();
			this.id = this.message.Id.ObjectId;
			this.newlyCreated = false;
		}

		internal SubscriptionCacheEntry AddSubscription(Guid tenantGuid, Guid externalDirectoryOrgId, AggregationSubscription subscription)
		{
			base.CheckDisposed();
			this.CheckSubscriptionsLoaded();
			SerializedSubscription serializedSubscription = SerializedSubscription.FromSubscription(subscription);
			SubscriptionCacheEntry cacheEntry = new SubscriptionCacheEntry(this.syncLogSession, subscription.SubscriptionGuid, subscription.SubscriptionMessageId, subscription.UserLegacyDN, this.mailboxGuid, tenantGuid, externalDirectoryOrgId, subscription.SubscriptionType, subscription.AggregationType, subscription.LastSyncTime, false, subscription.IncomingServerName, subscription.SyncPhase, serializedSubscription);
			return this.AddSubscription(cacheEntry);
		}

		internal SubscriptionCacheEntry AddSubscription(SubscriptionCacheEntry cacheEntry)
		{
			base.CheckDisposed();
			this.CheckSubscriptionsLoaded();
			if (this.FindSubscriptionBySubscriptionGuid(cacheEntry.SubscriptionGuid) != null)
			{
				return cacheEntry;
			}
			this.subscriptionCacheEntries.AddLast(cacheEntry);
			return cacheEntry;
		}

		internal SubscriptionCacheEntry RemoveSubscription(StoreObjectId subscriptionMessageId)
		{
			base.CheckDisposed();
			this.CheckSubscriptionsLoaded();
			foreach (SubscriptionCacheEntry subscriptionCacheEntry in this.subscriptionCacheEntries)
			{
				if (object.Equals(subscriptionMessageId, subscriptionCacheEntry.SubscriptionMessageId))
				{
					this.subscriptionCacheEntries.Remove(subscriptionCacheEntry);
					return subscriptionCacheEntry;
				}
			}
			return null;
		}

		internal void LoadSubscriptions()
		{
			base.CheckDisposed();
			if (this.loadedSubscriptions)
			{
				return;
			}
			this.loadedSubscriptions = true;
			using (Stream stream = this.OpenSubscriptionsDeserializationStream())
			{
				BinaryReader reader = new BinaryReader(stream);
				this.Deserialize(reader);
			}
		}

		protected virtual void SaveMessage()
		{
			this.message.Save(SaveMode.NoConflictResolution);
		}

		protected virtual void Serialize(BinaryWriter writer)
		{
			try
			{
				writer.Write(7);
				writer.Write(this.subscriptionListTimestamp.UtcTicks);
				if (this.subscriptionCacheEntries == null || this.subscriptionCacheEntries.Count == 0)
				{
					writer.Write(0);
				}
				else
				{
					writer.Write((byte)this.subscriptionCacheEntries.Count);
					foreach (SubscriptionCacheEntry subscriptionCacheEntry in this.subscriptionCacheEntries)
					{
						subscriptionCacheEntry.Serialize(writer);
					}
				}
			}
			catch (IOException innerException)
			{
				throw new SerializationException("Subscription Cache Entry is not valid.", innerException);
			}
		}

		protected virtual Stream OpenSubscriptionsSerializationStream()
		{
			return this.message.OpenPropertyStream(AggregationSubscriptionMessageSchema.SharingSubscriptionsCache, PropertyOpenMode.Create);
		}

		protected virtual Stream OpenSubscriptionsDeserializationStream()
		{
			return this.message.OpenPropertyStream(AggregationSubscriptionMessageSchema.SharingSubscriptionsCache, PropertyOpenMode.ReadOnly);
		}

		protected virtual void OpenMessageAsReadWrite()
		{
			this.message.OpenAsReadWrite();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.message != null)
			{
				this.message.Dispose();
				this.message = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SubscriptionCacheMessage>(this);
		}

		private void CheckSubscriptionsLoaded()
		{
			if (!this.loadedSubscriptions && !this.newlyCreated)
			{
				throw new InvalidOperationException("Run LoadSubscriptions first to load all subscriptions.");
			}
		}

		private void Deserialize(BinaryReader reader)
		{
			try
			{
				this.version = reader.ReadByte();
				if (this.version != 7)
				{
					throw new VersionMismatchException(string.Format(CultureInfo.InvariantCulture, "Invalid Subscription Cache version found in stream: {0}.", new object[]
					{
						this.version
					}));
				}
				long num = reader.ReadInt64();
				if (num < DateTime.MinValue.Ticks || num > DateTime.MaxValue.Ticks)
				{
					throw new SerializationException("Invalid subscriptionListTimestamp in ticks.");
				}
				this.subscriptionListTimestamp = new ExDateTime(ExTimeZone.UtcTimeZone, num);
				byte b = reader.ReadByte();
				for (byte b2 = 0; b2 < b; b2 += 1)
				{
					SubscriptionCacheEntry value = SubscriptionCacheEntry.FromSerialization(this.syncLogSession, reader, this.version);
					this.subscriptionCacheEntries.AddLast(value);
				}
			}
			catch (CorruptDataException innerException)
			{
				throw new SerializationException("Subscription Cache Entry is not valid.", innerException);
			}
			catch (IOException innerException2)
			{
				throw new SerializationException("Subscription Cache Entry is not valid.", innerException2);
			}
			catch (FormatException innerException3)
			{
				throw new SerializationException("Subscription Cache Entry is not valid.", innerException3);
			}
		}

		private const short InvalidVersion = -32768;

		private const byte CurrentVersion = 7;

		private static readonly Trace Tracer = ExTraceGlobals.SubscriptionCacheMessageTracer;

		private static readonly SortBy[] FindCacheMessageSortBySharingInstanceGuid = new SortBy[]
		{
			new SortBy(MessageItemSchema.SharingInstanceGuid, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] FindCacheMessageProperties = new PropertyDefinition[]
		{
			MessageItemSchema.SharingInstanceGuid,
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] BindProperties = new PropertyDefinition[]
		{
			MessageItemSchema.SharingInstanceGuid,
			AggregationSubscriptionMessageSchema.SharingSubscriptionConfiguration,
			AggregationSubscriptionMessageSchema.SharingSubscriptionsCache
		};

		private readonly GlobalSyncLogSession syncLogSession;

		private StoreObjectId id;

		private bool newlyCreated;

		private bool loadedSubscriptions;

		private MessageItem message;

		private Guid mailboxGuid;

		private ExDateTime subscriptionListTimestamp;

		private byte version;

		private LinkedList<SubscriptionCacheEntry> subscriptionCacheEntries;
	}
}
