using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class IdMapping : IIdMapping, ICustomSerializableBuilder, ICustomSerializable
	{
		public IdMapping()
		{
			this.syncIdToMailboxIdTable = new Dictionary<string, ISyncItemId>();
			this.mailboxIdToSyncIdTable = new Dictionary<ISyncItemId, string>();
			this.deletedItems = new List<string>();
			this.addedItems = new List<string>();
			this.uniqueCounter = 1L;
			this.oldIds = new Dictionary<ISyncItemId, string>();
		}

		public bool IsDirty
		{
			get
			{
				return this.dirty;
			}
		}

		public IDictionaryEnumerator MailboxIdIdEnumerator
		{
			get
			{
				return this.mailboxIdToSyncIdTable.GetEnumerator();
			}
		}

		public IDictionaryEnumerator SyncIdIdEnumerator
		{
			get
			{
				return this.syncIdToMailboxIdTable.GetEnumerator();
			}
		}

		public abstract ushort TypeId { get; set; }

		public bool UsingWriteBuffer
		{
			get
			{
				return this.usingWriteBuffer;
			}
		}

		protected Dictionary<ISyncItemId, string> OldIds
		{
			get
			{
				return this.oldIds;
			}
			set
			{
				this.oldIds = value;
			}
		}

		protected long UniqueCounter
		{
			get
			{
				return this.uniqueCounter;
			}
			set
			{
				this.uniqueCounter = value;
			}
		}

		public virtual ISyncItemId this[string syncId]
		{
			get
			{
				if (this.syncIdToMailboxIdTable.ContainsKey(syncId))
				{
					return this.syncIdToMailboxIdTable[syncId];
				}
				return null;
			}
		}

		public virtual string this[ISyncItemId mailboxId]
		{
			get
			{
				if (this.mailboxIdToSyncIdTable.ContainsKey(mailboxId))
				{
					return this.mailboxIdToSyncIdTable[mailboxId];
				}
				return null;
			}
		}

		public abstract ICustomSerializable BuildObject();

		public void ClearChanges()
		{
			for (int i = 0; i < this.addedItems.Count; i++)
			{
				string text = this.addedItems[i];
				ISyncItemId key = this.syncIdToMailboxIdTable[text];
				this.syncIdToMailboxIdTable.Remove(text);
				this.mailboxIdToSyncIdTable.Remove(key);
				this.oldIds[key] = text;
				this.dirty = true;
			}
			this.addedItems.Clear();
			if (this.deletedItems.Count > 0)
			{
				this.deletedItems.Clear();
				this.dirty = true;
			}
		}

		public void CommitChanges()
		{
			if (this.addedItems.Count > 0)
			{
				this.addedItems.Clear();
				this.dirty = true;
			}
			for (int i = 0; i < this.deletedItems.Count; i++)
			{
				string key = this.deletedItems[i];
				ISyncItemId key2 = this.syncIdToMailboxIdTable[key];
				this.mailboxIdToSyncIdTable.Remove(key2);
				this.syncIdToMailboxIdTable.Remove(key);
				this.dirty = true;
			}
			this.deletedItems.Clear();
		}

		public virtual bool Contains(ISyncItemId mailboxId)
		{
			return this.mailboxIdToSyncIdTable.ContainsKey(mailboxId) || (this.usingWriteBuffer && this.addedItemsWriteBufferReversed.ContainsKey(mailboxId));
		}

		public virtual bool Contains(string syncId)
		{
			return this.syncIdToMailboxIdTable.ContainsKey(syncId) || (this.usingWriteBuffer && this.addedItemsWriteBuffer.ContainsKey(syncId));
		}

		public void Delete(params ISyncItemId[] mailboxIds)
		{
			AirSyncDiagnostics.Assert(mailboxIds != null);
			for (int i = 0; i < mailboxIds.Length; i++)
			{
				if (this.mailboxIdToSyncIdTable.ContainsKey(mailboxIds[i]))
				{
					string item = this.mailboxIdToSyncIdTable[mailboxIds[i]];
					if (this.usingWriteBuffer)
					{
						if (!this.deletedItems.Contains(item) && !this.deletedItemsWriteBuffer.Contains(item))
						{
							this.deletedItemsWriteBuffer.Add(item);
						}
					}
					else if (!this.deletedItems.Contains(item))
					{
						this.deletedItems.Add(item);
						this.dirty = true;
					}
				}
				else if (this.usingWriteBuffer && this.addedItemsWriteBufferReversed.ContainsKey(mailboxIds[i]))
				{
					this.addedItemsWriteBuffer.Remove(this.addedItemsWriteBufferReversed[mailboxIds[i]]);
					this.addedItemsWriteBufferReversed.Remove(mailboxIds[i]);
				}
				else
				{
					AirSyncDiagnostics.Assert(false, "Id '{0}' is not found in the mapping", new object[]
					{
						mailboxIds[i]
					});
				}
			}
		}

		public void Delete(params string[] syncIds)
		{
			AirSyncDiagnostics.Assert(syncIds != null);
			for (int i = 0; i < syncIds.Length; i++)
			{
				if (this.syncIdToMailboxIdTable.ContainsKey(syncIds[i]))
				{
					if (this.usingWriteBuffer)
					{
						if (!this.deletedItems.Contains(syncIds[i]) && !this.deletedItemsWriteBuffer.Contains(syncIds[i]))
						{
							this.deletedItemsWriteBuffer.Add(syncIds[i]);
						}
					}
					else if (!this.deletedItems.Contains(syncIds[i]))
					{
						this.deletedItems.Add(syncIds[i]);
						this.dirty = true;
					}
				}
				else if (this.usingWriteBuffer && this.addedItemsWriteBuffer.ContainsKey(syncIds[i]))
				{
					this.addedItemsWriteBufferReversed.Remove(this.addedItemsWriteBuffer[syncIds[i]]);
					this.addedItemsWriteBuffer.Remove(syncIds[i]);
				}
				else
				{
					AirSyncDiagnostics.Assert(false, "Id '{0}' is not found in the mapping", new object[]
					{
						syncIds[i]
					});
				}
			}
		}

		public virtual void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (componentDataPool.ExternalVersion < 1)
			{
				GenericDictionaryData<StringData, string, StoreObjectIdData, StoreObjectId> genericDictionaryData = new GenericDictionaryData<StringData, string, StoreObjectIdData, StoreObjectId>();
				genericDictionaryData.DeserializeData(reader, componentDataPool);
				Dictionary<string, StoreObjectId> data = genericDictionaryData.Data;
				this.syncIdToMailboxIdTable = new Dictionary<string, ISyncItemId>(data.Count);
				using (Dictionary<string, StoreObjectId>.Enumerator enumerator = data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, StoreObjectId> keyValuePair = enumerator.Current;
						this.syncIdToMailboxIdTable[keyValuePair.Key] = MailboxSyncItemId.CreateForNewItem(keyValuePair.Value);
					}
					goto IL_92;
				}
			}
			GenericDictionaryData<StringData, string, DerivedData<ISyncItemId>, ISyncItemId> genericDictionaryData2 = new GenericDictionaryData<StringData, string, DerivedData<ISyncItemId>, ISyncItemId>();
			genericDictionaryData2.DeserializeData(reader, componentDataPool);
			this.syncIdToMailboxIdTable = genericDictionaryData2.Data;
			IL_92:
			this.mailboxIdToSyncIdTable = new Dictionary<ISyncItemId, string>(this.syncIdToMailboxIdTable.Count);
			foreach (KeyValuePair<string, ISyncItemId> keyValuePair2 in this.syncIdToMailboxIdTable)
			{
				this.mailboxIdToSyncIdTable.Add(keyValuePair2.Value, keyValuePair2.Key);
			}
			GenericListData<StringData, string> genericListData = new GenericListData<StringData, string>();
			genericListData.DeserializeData(reader, componentDataPool);
			this.deletedItems = genericListData.Data;
			genericListData.DeserializeData(reader, componentDataPool);
			this.addedItems = genericListData.Data;
			this.uniqueCounter = reader.ReadInt64();
		}

		public void Flush()
		{
			if (!this.usingWriteBuffer)
			{
				return;
			}
			this.usingWriteBuffer = false;
			foreach (string text in this.deletedItemsWriteBuffer)
			{
				this.Delete(new string[]
				{
					text
				});
			}
			this.deletedItemsWriteBuffer = null;
			foreach (string text2 in this.addedItemsWriteBuffer.Keys)
			{
				this.Add(this.addedItemsWriteBuffer[text2], text2);
			}
			this.addedItemsWriteBuffer = null;
		}

		public void IncreaseCounterTo(long newCount)
		{
			this.uniqueCounter = ((this.uniqueCounter > newCount) ? this.uniqueCounter : newCount);
		}

		public virtual void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			new GenericDictionaryData<StringData, string, DerivedData<ISyncItemId>, ISyncItemId>(this.syncIdToMailboxIdTable).SerializeData(writer, componentDataPool);
			new GenericListData<StringData, string>(this.deletedItems).SerializeData(writer, componentDataPool);
			new GenericListData<StringData, string>(this.addedItems).SerializeData(writer, componentDataPool);
			writer.Write(this.uniqueCounter);
		}

		public void UseWriteBuffer()
		{
			this.deletedItemsWriteBuffer = new List<string>(16);
			this.addedItemsWriteBuffer = new Dictionary<string, ISyncItemId>(16);
			this.addedItemsWriteBufferReversed = new Dictionary<ISyncItemId, string>(16);
			this.usingWriteBuffer = true;
		}

		protected void Add(ISyncItemId mailboxId, string syncId)
		{
			AirSyncDiagnostics.Assert(mailboxId != null);
			AirSyncDiagnostics.Assert(syncId != null);
			if (this.syncIdToMailboxIdTable.ContainsKey(syncId))
			{
				ISyncItemId syncItemId = this.syncIdToMailboxIdTable[syncId];
				if (syncItemId.Equals(mailboxId))
				{
					return;
				}
				throw new InvalidOperationException("SyncId has already been mapped to a different MailboxId");
			}
			else if (this.usingWriteBuffer && this.addedItemsWriteBuffer.ContainsKey(syncId))
			{
				ISyncItemId syncItemId2 = this.addedItemsWriteBuffer[syncId];
				if (syncItemId2.Equals(mailboxId))
				{
					return;
				}
				throw new InvalidOperationException("SyncId has already been mapped to a different MailboxId");
			}
			else
			{
				if (this.mailboxIdToSyncIdTable.ContainsKey(mailboxId) || (this.usingWriteBuffer && this.addedItemsWriteBufferReversed.ContainsKey(mailboxId)))
				{
					throw new InvalidOperationException("MailboxId has already been mapped to a different SyncId");
				}
				if (this.usingWriteBuffer)
				{
					this.addedItemsWriteBuffer[syncId] = mailboxId;
					this.addedItemsWriteBufferReversed[mailboxId] = syncId;
					this.uniqueCounter += 1L;
					return;
				}
				this.syncIdToMailboxIdTable.Add(syncId, mailboxId);
				this.mailboxIdToSyncIdTable.Add(mailboxId, syncId);
				this.addedItems.Add(syncId);
				this.uniqueCounter += 1L;
				this.dirty = true;
				return;
			}
		}

		protected bool IsInDeletedItemsBuffer(string syncId)
		{
			return this.deletedItems.Contains(syncId) || (this.usingWriteBuffer && this.deletedItemsWriteBuffer.Contains(syncId));
		}

		protected const int MaxSyncIdLength = 64;

		private List<string> addedItems;

		[NonSerialized]
		private Dictionary<string, ISyncItemId> addedItemsWriteBuffer;

		[NonSerialized]
		private Dictionary<ISyncItemId, string> addedItemsWriteBufferReversed;

		private List<string> deletedItems;

		[NonSerialized]
		private List<string> deletedItemsWriteBuffer;

		[NonSerialized]
		private bool dirty;

		private Dictionary<ISyncItemId, string> mailboxIdToSyncIdTable;

		private Dictionary<ISyncItemId, string> oldIds;

		private Dictionary<string, ISyncItemId> syncIdToMailboxIdTable;

		private long uniqueCounter;

		private bool usingWriteBuffer;
	}
}
