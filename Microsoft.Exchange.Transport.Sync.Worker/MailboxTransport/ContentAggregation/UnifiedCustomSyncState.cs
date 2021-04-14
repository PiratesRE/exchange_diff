using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class UnifiedCustomSyncState : ICustomSerializableBuilder, ICustomSerializable
	{
		internal UnifiedCustomSyncState()
		{
			this.cloudProperties = new Dictionary<string, string>(100);
			this.cloudItemTransientErrors = new Dictionary<string, short>(100);
			this.failedCloudItems = new Dictionary<string, string>(1);
			this.failedCloudFolders = new Dictionary<string, string>(1);
			this.cloudItemSyncConflictTransientErrors = new Dictionary<string, short>(1);
			this.nativeItemTransientErrors = new Dictionary<StoreObjectId, short>(100);
			this.nativeItemSyncConflictTransientErrors = new Dictionary<StoreObjectId, short>(1);
			this.cloudFolderTransientErrors = new Dictionary<string, short>(100);
			this.cloudFolderSyncConflictTransientErrors = new Dictionary<string, short>(1);
			this.nativeFolderTransientErrors = new Dictionary<StoreObjectId, short>(100);
			this.nativeFolderSyncConflictTransientErrors = new Dictionary<StoreObjectId, short>(1);
			this.nativeFolderMappingTable = new Dictionary<StoreObjectId, UnifiedCustomSyncStateItem>(100);
			this.cloudFolderMappingTable = new Dictionary<string, UnifiedCustomSyncStateItem>(100);
			this.nativeItemMappingTable = new Dictionary<StoreObjectId, UnifiedCustomSyncStateItem>(100);
			this.cloudItemMappingTable = new Dictionary<string, UnifiedCustomSyncStateItem>(100);
			this.version = 6;
		}

		public ushort TypeId
		{
			get
			{
				return UnifiedCustomSyncState.typeId;
			}
			set
			{
				UnifiedCustomSyncState.typeId = value;
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.dirty;
			}
		}

		internal bool InitialSyncDone
		{
			get
			{
				return (this.syncEngineFlags & 1) != 0;
			}
		}

		private static bool ShouldContinuePromotingTransientException(SyncTransientException exception)
		{
			return !SyncUtilities.VerifyNestedInnerExceptionType(exception, typeof(NonPromotableTransientException));
		}

		public ICustomSerializable BuildObject()
		{
			return new UnifiedCustomSyncState();
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(6);
			new GenericDictionaryData<StringData, string, StringData, string>(this.cloudProperties).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>(this.nativeItemTransientErrors).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>(this.nativeFolderTransientErrors).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StringData, string, Int16Data, short>(this.cloudItemTransientErrors).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StringData, string, Int16Data, short>(this.cloudFolderTransientErrors).SerializeData(writer, componentDataPool);
			bool flag = UnifiedCustomSyncState.RemoveOrphanedEntries(this.cloudFolderMappingTable, this.cloudFolderMappingTable);
			UnifiedCustomSyncState.SerializeCollection(this.cloudFolderMappingTable, writer, componentDataPool);
			if (flag)
			{
				UnifiedCustomSyncState.RemoveOrphanedEntries(this.cloudFolderMappingTable, this.cloudItemMappingTable);
			}
			UnifiedCustomSyncState.SerializeCollection(this.cloudItemMappingTable, writer, componentDataPool);
			new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>(this.nativeItemSyncConflictTransientErrors).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>(this.nativeFolderSyncConflictTransientErrors).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StringData, string, Int16Data, short>(this.cloudItemSyncConflictTransientErrors).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StringData, string, Int16Data, short>(this.cloudFolderSyncConflictTransientErrors).SerializeData(writer, componentDataPool);
			writer.Write(this.syncEngineFlags);
			new GenericDictionaryData<StringData, string, StringData, string>(this.failedCloudItems).SerializeData(writer, componentDataPool);
			new GenericDictionaryData<StringData, string, StringData, string>(this.failedCloudFolders).SerializeData(writer, componentDataPool);
			this.dirty = false;
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			this.version = reader.ReadInt16();
			if (this.version > 6)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid Unified Custom Sync State version found in stream: {0}.", new object[]
				{
					this.version
				}));
			}
			GenericDictionaryData<StringData, string, StringData, string> genericDictionaryData = new GenericDictionaryData<StringData, string, StringData, string>();
			genericDictionaryData.DeserializeData(reader, componentDataPool);
			this.cloudProperties = genericDictionaryData.Data;
			GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short> genericDictionaryData2 = new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>();
			genericDictionaryData2.DeserializeData(reader, componentDataPool);
			this.existingNativeItemTransientErrors = genericDictionaryData2.Data;
			GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short> genericDictionaryData3 = new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>();
			genericDictionaryData3.DeserializeData(reader, componentDataPool);
			this.existingNativeFolderTransientErrors = genericDictionaryData3.Data;
			GenericDictionaryData<StringData, string, Int16Data, short> genericDictionaryData4 = new GenericDictionaryData<StringData, string, Int16Data, short>();
			genericDictionaryData4.DeserializeData(reader, componentDataPool);
			this.existingCloudItemTransientErrors = genericDictionaryData4.Data;
			GenericDictionaryData<StringData, string, Int16Data, short> genericDictionaryData5 = new GenericDictionaryData<StringData, string, Int16Data, short>();
			genericDictionaryData5.DeserializeData(reader, componentDataPool);
			this.existingCloudFolderTransientErrors = genericDictionaryData5.Data;
			UnifiedCustomSyncState.DeserializeCollections(this.version, reader, componentDataPool, true, out this.nativeFolderMappingTable, out this.cloudFolderMappingTable);
			UnifiedCustomSyncState.DeserializeCollections(this.version, reader, componentDataPool, false, out this.nativeItemMappingTable, out this.cloudItemMappingTable);
			if (this.version == 0)
			{
				reader.ReadBoolean();
				reader.ReadBoolean();
			}
			if (this.version >= 2)
			{
				GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short> genericDictionaryData6 = new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>();
				genericDictionaryData6.DeserializeData(reader, componentDataPool);
				this.existingNativeItemSyncConflictTransientErrors = genericDictionaryData6.Data;
				GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short> genericDictionaryData7 = new GenericDictionaryData<StoreObjectIdData, StoreObjectId, Int16Data, short>();
				genericDictionaryData7.DeserializeData(reader, componentDataPool);
				this.existingNativeFolderSyncConflictTransientErrors = genericDictionaryData7.Data;
				GenericDictionaryData<StringData, string, Int16Data, short> genericDictionaryData8 = new GenericDictionaryData<StringData, string, Int16Data, short>();
				genericDictionaryData8.DeserializeData(reader, componentDataPool);
				this.existingCloudItemSyncConflictTransientErrors = genericDictionaryData8.Data;
				GenericDictionaryData<StringData, string, Int16Data, short> genericDictionaryData9 = new GenericDictionaryData<StringData, string, Int16Data, short>();
				genericDictionaryData9.DeserializeData(reader, componentDataPool);
				this.existingCloudFolderSyncConflictTransientErrors = genericDictionaryData9.Data;
			}
			if (this.version >= 4)
			{
				this.syncEngineFlags = reader.ReadByte();
			}
			if (this.version >= 5)
			{
				GenericDictionaryData<StringData, string, StringData, string> genericDictionaryData10 = new GenericDictionaryData<StringData, string, StringData, string>();
				genericDictionaryData10.DeserializeData(reader, componentDataPool);
				this.failedCloudItems = genericDictionaryData10.Data;
			}
			if (this.version >= 6)
			{
				GenericDictionaryData<StringData, string, StringData, string> genericDictionaryData11 = new GenericDictionaryData<StringData, string, StringData, string>();
				genericDictionaryData11.DeserializeData(reader, componentDataPool);
				this.failedCloudFolders = genericDictionaryData11.Data;
			}
			this.dirty = false;
		}

		internal void MarkInitialSyncDone()
		{
			this.syncEngineFlags |= 1;
			this.dirty = true;
		}

		internal bool ContainsItem(string cloudId)
		{
			return this.cloudItemMappingTable.ContainsKey(cloudId);
		}

		internal bool ContainsFailedItem(string cloudId)
		{
			return this.failedCloudItems.ContainsKey(cloudId);
		}

		internal bool ContainsFolder(StoreObjectId nativeId)
		{
			return this.nativeFolderMappingTable.ContainsKey(nativeId);
		}

		internal bool ContainsFailedFolder(string cloudId)
		{
			return this.failedCloudFolders.ContainsKey(cloudId);
		}

		internal bool ContainsFolder(string cloudId)
		{
			return this.cloudFolderMappingTable.ContainsKey(cloudId);
		}

		internal bool ContainsItem(StoreObjectId nativeId)
		{
			return this.nativeItemMappingTable.ContainsKey(nativeId);
		}

		internal bool TryAddItem(string cloudId, string cloudFolderId, StoreObjectId nativeId, byte[] changeKey, StoreObjectId nativeFolderId, string cloudVersion, Dictionary<string, string> itemProperties)
		{
			if (cloudId == null)
			{
				throw new ArgumentNullException("cloudId");
			}
			if (nativeId != null && nativeFolderId == null)
			{
				throw new ArgumentNullException("nativeFolderId");
			}
			UnifiedCustomSyncStateItem value = new UnifiedCustomSyncStateItem(nativeId, changeKey, nativeFolderId, cloudId, cloudFolderId, cloudVersion, itemProperties, this.version);
			if (this.cloudItemMappingTable.ContainsKey(cloudId))
			{
				return false;
			}
			if (nativeId != null)
			{
				if (this.nativeItemMappingTable.ContainsKey(nativeId))
				{
					return false;
				}
				this.nativeItemMappingTable.Add(nativeId, value);
			}
			this.cloudItemMappingTable.Add(cloudId, value);
			this.dirty = true;
			return true;
		}

		internal bool TryAddFailedItem(string cloudId, string cloudFolderId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("cloudId", cloudId);
			if (this.failedCloudItems.ContainsKey(cloudId))
			{
				return false;
			}
			this.failedCloudItems.Add(cloudId, cloudFolderId);
			this.dirty = true;
			return true;
		}

		internal bool TryFindItem(string cloudId, out string cloudFolderId, out StoreObjectId nativeId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties)
		{
			return UnifiedCustomSyncState.TryFindCollection<string>(this.cloudItemMappingTable, cloudId, out cloudId, out cloudFolderId, out nativeId, out changeKey, out nativeFolderId, out cloudVersion, out itemProperties);
		}

		internal bool TryFindItem(StoreObjectId nativeId, out string cloudId, out string cloudFolderId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties)
		{
			return UnifiedCustomSyncState.TryFindCollection<StoreObjectId>(this.nativeItemMappingTable, nativeId, out cloudId, out cloudFolderId, out nativeId, out changeKey, out nativeFolderId, out cloudVersion, out itemProperties);
		}

		internal bool TryUpdateItem(StoreObjectId nativeId, byte[] changeKey, string cloudVersion, Dictionary<string, string> itemProperties)
		{
			return this.TryUpdateCollection<StoreObjectId>(this.nativeItemMappingTable, nativeId, changeKey, cloudVersion, itemProperties);
		}

		internal bool TryRemoveItem(string cloudId)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (this.cloudItemMappingTable.TryGetValue(cloudId, out unifiedCustomSyncStateItem) && unifiedCustomSyncStateItem.NativeId != null)
			{
				this.nativeItemMappingTable.Remove(unifiedCustomSyncStateItem.NativeId);
			}
			if (this.cloudItemMappingTable.Remove(cloudId))
			{
				this.dirty = true;
				return true;
			}
			return false;
		}

		internal bool TryRemoveFailedItem(string cloudId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("cloudId", cloudId);
			if (this.failedCloudItems.Remove(cloudId))
			{
				this.dirty = true;
				return true;
			}
			return false;
		}

		internal bool TryRemoveItem(StoreObjectId nativeId)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (this.nativeItemMappingTable.TryGetValue(nativeId, out unifiedCustomSyncStateItem) && unifiedCustomSyncStateItem.CloudId != null)
			{
				this.cloudItemMappingTable.Remove(unifiedCustomSyncStateItem.CloudId);
			}
			if (this.nativeItemMappingTable.Remove(nativeId))
			{
				this.dirty = true;
				return true;
			}
			return false;
		}

		internal bool TryAddFolder(bool isInbox, string cloudId, string cloudFolderId, StoreObjectId nativeId, byte[] changeKey, StoreObjectId nativeFolderId, string cloudVersion, Dictionary<string, string> folderProperties)
		{
			if (cloudId == null)
			{
				throw new ArgumentNullException("cloudId");
			}
			UnifiedCustomSyncStateItem value = new UnifiedCustomSyncStateItem(nativeId, changeKey, nativeFolderId, cloudId, cloudFolderId, cloudVersion, folderProperties, this.version);
			if (this.cloudFolderMappingTable.ContainsKey(cloudId))
			{
				return false;
			}
			if (nativeId != null && !isInbox)
			{
				if (this.nativeFolderMappingTable.ContainsKey(nativeId))
				{
					return false;
				}
				this.nativeFolderMappingTable.Add(nativeId, value);
			}
			this.cloudFolderMappingTable.Add(cloudId, value);
			this.dirty = true;
			return true;
		}

		internal bool TryAddFailedFolder(string cloudId, string cloudFolderId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("cloudId", cloudId);
			if (this.failedCloudFolders.ContainsKey(cloudId))
			{
				return false;
			}
			this.failedCloudFolders.Add(cloudId, cloudFolderId);
			this.dirty = true;
			return true;
		}

		internal bool TryFindFolder(string cloudId, out string cloudFolderId, out StoreObjectId nativeId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties)
		{
			return UnifiedCustomSyncState.TryFindCollection<string>(this.cloudFolderMappingTable, cloudId, out cloudId, out cloudFolderId, out nativeId, out changeKey, out nativeFolderId, out cloudVersion, out folderProperties);
		}

		internal bool TryFindFolder(StoreObjectId nativeId, out string cloudId, out string cloudFolderId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties)
		{
			return UnifiedCustomSyncState.TryFindCollection<StoreObjectId>(this.nativeFolderMappingTable, nativeId, out cloudId, out cloudFolderId, out nativeId, out changeKey, out nativeFolderId, out cloudVersion, out folderProperties);
		}

		internal bool TryUpdateFolder(bool isInbox, StoreObjectId nativeId, StoreObjectId newNativeId, string cloudId, string newCloudId, string newCloudFolderId, byte[] changeKey, StoreObjectId newNativeFolderId, string cloudVersion, Dictionary<string, string> folderProperties)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (!this.nativeFolderMappingTable.TryGetValue(nativeId, out unifiedCustomSyncStateItem) && !this.cloudFolderMappingTable.TryGetValue(cloudId, out unifiedCustomSyncStateItem))
			{
				return false;
			}
			this.dirty = true;
			unifiedCustomSyncStateItem.ChangeKey = changeKey;
			unifiedCustomSyncStateItem.CloudVersion = cloudVersion;
			unifiedCustomSyncStateItem.Properties = folderProperties;
			if (newNativeFolderId != null)
			{
				unifiedCustomSyncStateItem.NativeFolderId = newNativeFolderId;
			}
			if (newCloudFolderId != null)
			{
				unifiedCustomSyncStateItem.CloudFolderId = newCloudFolderId;
			}
			if (newCloudId != null)
			{
				int hashCode = unifiedCustomSyncStateItem.CloudId.GetHashCode();
				foreach (UnifiedCustomSyncStateItem unifiedCustomSyncStateItem2 in this.cloudFolderMappingTable.Values)
				{
					if (unifiedCustomSyncStateItem2.CloudFolderId != null && unifiedCustomSyncStateItem2.CloudFolderId.GetHashCode() == hashCode)
					{
						unifiedCustomSyncStateItem2.CloudFolderId = newCloudId;
					}
				}
				foreach (UnifiedCustomSyncStateItem unifiedCustomSyncStateItem3 in this.cloudItemMappingTable.Values)
				{
					if (unifiedCustomSyncStateItem3.CloudFolderId != null && unifiedCustomSyncStateItem3.CloudFolderId.GetHashCode() == hashCode)
					{
						unifiedCustomSyncStateItem3.CloudFolderId = newCloudId;
					}
				}
				this.cloudFolderMappingTable.Remove(unifiedCustomSyncStateItem.CloudId);
				unifiedCustomSyncStateItem.CloudId = newCloudId;
				this.cloudFolderMappingTable.Add(newCloudId, unifiedCustomSyncStateItem);
			}
			if (newNativeId != null)
			{
				this.UpdateFolderNativeId(isInbox, nativeId, newNativeId, unifiedCustomSyncStateItem);
			}
			return true;
		}

		internal bool TryUpdateFolder(bool isInbox, StoreObjectId nativeId, StoreObjectId newNativeId)
		{
			UnifiedCustomSyncStateItem item;
			if (!this.nativeFolderMappingTable.TryGetValue(nativeId, out item))
			{
				return false;
			}
			this.dirty = true;
			this.UpdateFolderNativeId(isInbox, nativeId, newNativeId, item);
			return true;
		}

		private static bool ShouldPromoteTransientException<T>(Dictionary<T, short> currentTransientErrorCollection, Dictionary<T, short> previousTransientErrorCollection, T id, int maxTransientErrors)
		{
			short num;
			if (currentTransientErrorCollection.TryGetValue(id, out num))
			{
				num += 1;
				currentTransientErrorCollection[id] = num;
			}
			else
			{
				if (previousTransientErrorCollection != null && previousTransientErrorCollection.TryGetValue(id, out num))
				{
					num += 1;
				}
				else
				{
					num = 1;
				}
				currentTransientErrorCollection.Add(id, num);
			}
			return (int)num >= maxTransientErrors;
		}

		private static void SerializeCollection(Dictionary<string, UnifiedCustomSyncStateItem> cloudCollection, BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			writer.Write(cloudCollection != null && cloudCollection.Count > 0);
			if (cloudCollection != null && cloudCollection.Count > 0)
			{
				writer.Write(cloudCollection.Count);
				foreach (UnifiedCustomSyncStateItem unifiedCustomSyncStateItem in cloudCollection.Values)
				{
					unifiedCustomSyncStateItem.SerializeData(writer, componentDataPool);
				}
			}
		}

		private static bool RemoveOrphanedEntries(Dictionary<string, UnifiedCustomSyncStateItem> folderCollection, Dictionary<string, UnifiedCustomSyncStateItem> itemCollection)
		{
			bool result = false;
			LinkedList<UnifiedCustomSyncStateItem> linkedList = null;
			foreach (UnifiedCustomSyncStateItem unifiedCustomSyncStateItem in itemCollection.Values)
			{
				if (unifiedCustomSyncStateItem.CloudFolderId != null && !folderCollection.ContainsKey(unifiedCustomSyncStateItem.CloudFolderId))
				{
					if (linkedList == null)
					{
						linkedList = new LinkedList<UnifiedCustomSyncStateItem>();
					}
					linkedList.AddLast(unifiedCustomSyncStateItem);
				}
			}
			if (linkedList != null)
			{
				foreach (UnifiedCustomSyncStateItem unifiedCustomSyncStateItem2 in linkedList)
				{
					itemCollection.Remove(unifiedCustomSyncStateItem2.CloudId);
					result = true;
				}
			}
			return result;
		}

		private static void DeserializeCollections(short version, BinaryReader reader, ComponentDataPool componentDataPool, bool skipMultipleReferencesToNativeId, out Dictionary<StoreObjectId, UnifiedCustomSyncStateItem> nativeCollection, out Dictionary<string, UnifiedCustomSyncStateItem> cloudCollection)
		{
			if (reader.ReadBoolean())
			{
				int num = reader.ReadInt32();
				nativeCollection = new Dictionary<StoreObjectId, UnifiedCustomSyncStateItem>(num);
				cloudCollection = new Dictionary<string, UnifiedCustomSyncStateItem>(num);
				for (int i = 0; i < num; i++)
				{
					UnifiedCustomSyncStateItem unifiedCustomSyncStateItem = new UnifiedCustomSyncStateItem(version);
					unifiedCustomSyncStateItem.DeserializeData(reader, componentDataPool);
					if (unifiedCustomSyncStateItem.NativeId != null && (!nativeCollection.ContainsKey(unifiedCustomSyncStateItem.NativeId) || !skipMultipleReferencesToNativeId))
					{
						nativeCollection.Add(unifiedCustomSyncStateItem.NativeId, unifiedCustomSyncStateItem);
					}
					cloudCollection.Add(unifiedCustomSyncStateItem.CloudId, unifiedCustomSyncStateItem);
				}
				return;
			}
			nativeCollection = new Dictionary<StoreObjectId, UnifiedCustomSyncStateItem>(100);
			cloudCollection = new Dictionary<string, UnifiedCustomSyncStateItem>(100);
		}

		private static bool TryFindCollection<T>(Dictionary<T, UnifiedCustomSyncStateItem> collection, T id, out string cloudId, out string cloudFolderId, out StoreObjectId nativeId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> properties)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (!collection.TryGetValue(id, out unifiedCustomSyncStateItem))
			{
				cloudId = null;
				cloudFolderId = null;
				nativeId = null;
				changeKey = null;
				nativeFolderId = null;
				cloudVersion = null;
				properties = null;
				return false;
			}
			cloudId = unifiedCustomSyncStateItem.CloudId;
			cloudFolderId = unifiedCustomSyncStateItem.CloudFolderId;
			nativeId = unifiedCustomSyncStateItem.NativeId;
			changeKey = unifiedCustomSyncStateItem.ChangeKey;
			nativeFolderId = unifiedCustomSyncStateItem.NativeFolderId;
			cloudVersion = unifiedCustomSyncStateItem.CloudVersion;
			properties = unifiedCustomSyncStateItem.Properties;
			return true;
		}

		private void UpdateFolderNativeId(bool isInbox, StoreObjectId nativeId, StoreObjectId newNativeId, UnifiedCustomSyncStateItem item)
		{
			int hashCode = nativeId.GetHashCode();
			foreach (UnifiedCustomSyncStateItem unifiedCustomSyncStateItem in this.nativeFolderMappingTable.Values)
			{
				if (unifiedCustomSyncStateItem.NativeFolderId != null && unifiedCustomSyncStateItem.NativeFolderId.GetHashCode() == hashCode)
				{
					unifiedCustomSyncStateItem.NativeFolderId = newNativeId;
				}
			}
			foreach (UnifiedCustomSyncStateItem unifiedCustomSyncStateItem2 in this.nativeItemMappingTable.Values)
			{
				if (unifiedCustomSyncStateItem2.NativeFolderId != null && unifiedCustomSyncStateItem2.NativeFolderId.GetHashCode() == hashCode)
				{
					unifiedCustomSyncStateItem2.NativeFolderId = newNativeId;
				}
			}
			this.nativeFolderMappingTable.Remove(nativeId);
			item.NativeId = newNativeId;
			if (newNativeId != null && !isInbox)
			{
				this.nativeFolderMappingTable.Add(newNativeId, item);
			}
		}

		internal bool TryRemoveFolder(string cloudId)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (this.cloudFolderMappingTable.TryGetValue(cloudId, out unifiedCustomSyncStateItem) && unifiedCustomSyncStateItem.NativeId != null)
			{
				this.nativeItemMappingTable.Remove(unifiedCustomSyncStateItem.NativeId);
			}
			if (this.cloudFolderMappingTable.Remove(cloudId))
			{
				UnifiedCustomSyncState.RemoveOrphanedEntries(this.cloudFolderMappingTable, this.cloudItemMappingTable);
				this.dirty = true;
				return true;
			}
			return false;
		}

		internal bool TryRemoveFailedFolder(string cloudId)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("cloudId", cloudId);
			if (this.failedCloudFolders.Remove(cloudId))
			{
				this.dirty = true;
				return true;
			}
			return false;
		}

		internal bool TryRemoveFolder(StoreObjectId nativeId)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (this.nativeFolderMappingTable.TryGetValue(nativeId, out unifiedCustomSyncStateItem) && unifiedCustomSyncStateItem.CloudId != null)
			{
				this.cloudFolderMappingTable.Remove(unifiedCustomSyncStateItem.CloudId);
			}
			if (this.nativeFolderMappingTable.Remove(nativeId))
			{
				UnifiedCustomSyncState.RemoveOrphanedEntries(this.cloudFolderMappingTable, this.cloudItemMappingTable);
				this.dirty = true;
				return true;
			}
			return false;
		}

		internal IEnumerator<string> GetCloudItemEnumerator()
		{
			return this.cloudItemMappingTable.Keys.GetEnumerator();
		}

		internal IEnumerator<string> GetFailedCloudItemEnumerator()
		{
			return this.failedCloudItems.Keys.GetEnumerator();
		}

		internal IEnumerator<string> GetFailedCloudItemFilteredByCloudFolderIdEnumerator(string cloudFolderId)
		{
			return new FailedItemEnumeratorInCloudFolder(this.failedCloudItems.GetEnumerator(), cloudFolderId);
		}

		internal IEnumerator<string> GetCloudFolderEnumerator()
		{
			return this.cloudFolderMappingTable.Keys.GetEnumerator();
		}

		internal IEnumerator<StoreObjectId> GetNativeItemEnumerator()
		{
			return this.nativeItemMappingTable.Keys.GetEnumerator();
		}

		internal IEnumerator<StoreObjectId> GetNativeFolderEnumerator()
		{
			return this.nativeFolderMappingTable.Keys.GetEnumerator();
		}

		internal IEnumerator<string> GetCloudItemFilteredByCloudFolderIdEnumerator(string cloudFolderId)
		{
			return new EnumeratorInCloudFolder(this.cloudItemMappingTable.Values.GetEnumerator(), cloudFolderId);
		}

		internal IEnumerator<string> GetCloudFolderFilteredByCloudFolderIdEnumerator(string cloudFolderId)
		{
			return new EnumeratorInCloudFolder(this.cloudFolderMappingTable.Values.GetEnumerator(), cloudFolderId);
		}

		internal IEnumerator<StoreObjectId> GetNativeItemFilteredByNativeFolderIdEnumerator(StoreObjectId nativeFolderId)
		{
			return new EnumeratorInNativeFolder(this.nativeItemMappingTable.Values.GetEnumerator(), nativeFolderId);
		}

		internal IEnumerator<StoreObjectId> GetNativeFolderFilteredByNativeFolderIdEnumerator(StoreObjectId nativeFolderId)
		{
			return new EnumeratorInNativeFolder(this.nativeFolderMappingTable.Values.GetEnumerator(), nativeFolderId);
		}

		internal bool TryUpdateItemCloudVersion(string cloudId, string cloudVersion)
		{
			return this.TryUpdateCollectionCloudVersion<string>(this.cloudItemMappingTable, cloudId, cloudVersion);
		}

		internal bool TryUpdateFolderCloudVersion(string cloudId, string cloudVersion)
		{
			return this.TryUpdateCollectionCloudVersion<string>(this.cloudFolderMappingTable, cloudId, cloudVersion);
		}

		internal void AddProperty(string property, string value)
		{
			this.cloudProperties.Add(property, value);
			this.dirty = true;
		}

		internal bool TryGetPropertyValue(string property, out string value)
		{
			return this.cloudProperties.TryGetValue(property, out value);
		}

		internal bool TryRemoveProperty(string property)
		{
			bool flag = this.cloudProperties.Remove(property);
			if (flag)
			{
				this.dirty = true;
			}
			return flag;
		}

		internal void ChangePropertyValue(string property, string value)
		{
			this.cloudProperties[property] = value;
			this.dirty = true;
		}

		internal bool ContainsProperty(string property)
		{
			return this.cloudProperties.ContainsKey(property);
		}

		internal bool ShouldPromoteItemTransientException(string cloudId, SyncTransientException exception)
		{
			if (!UnifiedCustomSyncState.ShouldContinuePromotingTransientException(exception))
			{
				return false;
			}
			this.dirty = true;
			if (SyncUtilities.VerifyNestedInnerExceptionType(exception, typeof(SyncConflictException)))
			{
				return UnifiedCustomSyncState.ShouldPromoteTransientException<string>(this.cloudItemSyncConflictTransientErrors, this.existingCloudItemSyncConflictTransientErrors, cloudId, 24);
			}
			return UnifiedCustomSyncState.ShouldPromoteTransientException<string>(this.cloudItemTransientErrors, this.existingCloudItemTransientErrors, cloudId, UnifiedCustomSyncState.MaxTransientErrors);
		}

		internal bool ShouldPromoteItemTransientException(StoreObjectId nativeId, SyncTransientException exception)
		{
			if (!UnifiedCustomSyncState.ShouldContinuePromotingTransientException(exception))
			{
				return false;
			}
			this.dirty = true;
			if (SyncUtilities.VerifyNestedInnerExceptionType(exception, typeof(SyncConflictException)))
			{
				return UnifiedCustomSyncState.ShouldPromoteTransientException<StoreObjectId>(this.nativeItemSyncConflictTransientErrors, this.existingNativeItemSyncConflictTransientErrors, nativeId, 24);
			}
			return UnifiedCustomSyncState.ShouldPromoteTransientException<StoreObjectId>(this.nativeItemTransientErrors, this.existingNativeItemTransientErrors, nativeId, UnifiedCustomSyncState.MaxTransientErrors);
		}

		internal bool ShouldPromoteFolderTransientException(string cloudId, SyncTransientException exception)
		{
			if (!UnifiedCustomSyncState.ShouldContinuePromotingTransientException(exception))
			{
				return false;
			}
			this.dirty = true;
			if (SyncUtilities.VerifyNestedInnerExceptionType(exception, typeof(SyncConflictException)))
			{
				return UnifiedCustomSyncState.ShouldPromoteTransientException<string>(this.cloudFolderSyncConflictTransientErrors, this.existingCloudFolderSyncConflictTransientErrors, cloudId, 24);
			}
			return UnifiedCustomSyncState.ShouldPromoteTransientException<string>(this.cloudFolderTransientErrors, this.existingCloudFolderTransientErrors, cloudId, UnifiedCustomSyncState.MaxTransientErrors);
		}

		internal bool ShouldPromoteFolderTransientException(StoreObjectId nativeId, SyncTransientException exception)
		{
			if (!UnifiedCustomSyncState.ShouldContinuePromotingTransientException(exception))
			{
				return false;
			}
			this.dirty = true;
			if (SyncUtilities.VerifyNestedInnerExceptionType(exception, typeof(SyncConflictException)))
			{
				return UnifiedCustomSyncState.ShouldPromoteTransientException<StoreObjectId>(this.nativeFolderSyncConflictTransientErrors, this.existingNativeFolderSyncConflictTransientErrors, nativeId, 24);
			}
			return UnifiedCustomSyncState.ShouldPromoteTransientException<StoreObjectId>(this.nativeFolderTransientErrors, this.existingNativeFolderTransientErrors, nativeId, UnifiedCustomSyncState.MaxTransientErrors);
		}

		internal IEnumerator<string> GetItemEnumerator()
		{
			return this.cloudItemMappingTable.Keys.GetEnumerator();
		}

		internal IEnumerator<string> GetFolderEnumerator()
		{
			return this.cloudFolderMappingTable.Keys.GetEnumerator();
		}

		private bool TryUpdateCollection<T>(Dictionary<T, UnifiedCustomSyncStateItem> collection, T id, byte[] changeKey, string cloudVersion, Dictionary<string, string> properties)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (!collection.TryGetValue(id, out unifiedCustomSyncStateItem))
			{
				return false;
			}
			this.dirty = true;
			unifiedCustomSyncStateItem.ChangeKey = changeKey;
			unifiedCustomSyncStateItem.CloudVersion = cloudVersion;
			unifiedCustomSyncStateItem.Properties = properties;
			return true;
		}

		private bool TryUpdateCollectionCloudVersion<T>(Dictionary<T, UnifiedCustomSyncStateItem> collection, T id, string cloudVersion)
		{
			UnifiedCustomSyncStateItem unifiedCustomSyncStateItem;
			if (!collection.TryGetValue(id, out unifiedCustomSyncStateItem))
			{
				return false;
			}
			this.dirty = true;
			unifiedCustomSyncStateItem.CloudVersion = cloudVersion;
			return true;
		}

		internal const short InvalidVersion = -32768;

		private const short MaxSyncConflictTransientErrors = 24;

		private const short CurrentVersion = 6;

		private const int DefaultSyncMappingSize = 100;

		private const int DefaultSyncConflictErrorListSize = 1;

		private const int DefaultFailedItemsListSize = 1;

		private const int DefaultFailedFoldersListSize = 1;

		internal static readonly int MaxTransientErrors = AggregationConfiguration.Instance.MaxTransientErrorsPerItem;

		private static ushort typeId;

		private bool dirty;

		private short version;

		private Dictionary<string, string> cloudProperties;

		private Dictionary<string, short> cloudItemTransientErrors;

		private Dictionary<string, string> failedCloudItems;

		private Dictionary<string, string> failedCloudFolders;

		private Dictionary<string, short> cloudItemSyncConflictTransientErrors;

		private Dictionary<StoreObjectId, short> nativeItemTransientErrors;

		private Dictionary<StoreObjectId, short> nativeItemSyncConflictTransientErrors;

		private Dictionary<string, short> cloudFolderTransientErrors;

		private Dictionary<string, short> cloudFolderSyncConflictTransientErrors;

		private Dictionary<StoreObjectId, short> nativeFolderTransientErrors;

		private Dictionary<StoreObjectId, short> nativeFolderSyncConflictTransientErrors;

		private byte syncEngineFlags;

		private Dictionary<StoreObjectId, UnifiedCustomSyncStateItem> nativeFolderMappingTable;

		private Dictionary<string, UnifiedCustomSyncStateItem> cloudFolderMappingTable;

		private Dictionary<StoreObjectId, UnifiedCustomSyncStateItem> nativeItemMappingTable;

		private Dictionary<string, UnifiedCustomSyncStateItem> cloudItemMappingTable;

		private Dictionary<string, short> existingCloudItemTransientErrors;

		private Dictionary<string, short> existingCloudItemSyncConflictTransientErrors;

		private Dictionary<StoreObjectId, short> existingNativeItemTransientErrors;

		private Dictionary<StoreObjectId, short> existingNativeItemSyncConflictTransientErrors;

		private Dictionary<string, short> existingCloudFolderTransientErrors;

		private Dictionary<string, short> existingCloudFolderSyncConflictTransientErrors;

		private Dictionary<StoreObjectId, short> existingNativeFolderTransientErrors;

		private Dictionary<StoreObjectId, short> existingNativeFolderSyncConflictTransientErrors;

		[Flags]
		private enum SyncEngineFlagsMask : byte
		{
			InitialSyncDone = 1
		}
	}
}
