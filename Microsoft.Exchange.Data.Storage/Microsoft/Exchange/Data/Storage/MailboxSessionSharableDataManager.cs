using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxSessionSharableDataManager
	{
		public MailboxSessionSharableDataManager()
		{
			this.defaultFolderData = new DefaultFolderData[DefaultFolderInfo.DefaultFolderTypeCount];
			for (int i = 0; i < this.defaultFolderData.Length; i++)
			{
				this.defaultFolderData[i] = new DefaultFolderData(false);
			}
		}

		internal bool DefaultFoldersInitialized
		{
			get
			{
				return this.defaultFoldersInitialized;
			}
			set
			{
				if (this.defaultFoldersInitialized != value && this.defaultFoldersInitialized)
				{
					throw new InvalidOperationException("Default folders already initialized.");
				}
				this.defaultFoldersInitialized = value;
			}
		}

		internal DefaultFolderData GetDefaultFolder(DefaultFolderType defaultFolderType)
		{
			return this.defaultFolderData[(int)defaultFolderType];
		}

		internal void SetDefaultFolder(DefaultFolderType defaultFolderType, DefaultFolderData defaultFolderData)
		{
			this.defaultFolderData[(int)defaultFolderType] = defaultFolderData;
		}

		internal CultureInfo DefaultFoldersCulture { get; set; }

		internal bool HasCacheEntryLoaded
		{
			get
			{
				return this.hasUserConfigurationLoaded;
			}
		}

		internal bool HasCacheEntries
		{
			get
			{
				bool result;
				lock (this.userConfigCacheEntryList)
				{
					result = (this.userConfigCacheEntryList.Count > 0);
				}
				return result;
			}
		}

		internal void Remove(UserConfigurationName configName, StoreObjectId folderId)
		{
			LinkedListNode<UserConfigurationCacheEntry> linkedListNode = this.userConfigCacheEntryList.First;
			lock (this.userConfigCacheEntryList)
			{
				while (linkedListNode != null)
				{
					UserConfigurationCacheEntry value = linkedListNode.Value;
					if (value.CheckMatch(configName.Name, folderId))
					{
						this.userConfigCacheEntryList.Remove(linkedListNode);
						break;
					}
					linkedListNode = linkedListNode.Next;
				}
			}
		}

		internal void ParseUserConfigurationCacheEntries(BinaryReader reader)
		{
			reader.ReadUInt16();
			int num = reader.ReadInt32();
			this.hasUserConfigurationLoaded = true;
			if (num <= 32)
			{
				for (int i = 0; i < num; i++)
				{
					string configName = reader.ReadString();
					int count = reader.ReadInt32();
					byte[] byteArray = reader.ReadBytes(count);
					count = reader.ReadInt32();
					byte[] byteArray2 = reader.ReadBytes(count);
					this.AddUserConfigurationCachedEntry(StoreObjectId.Parse(byteArray, 0), configName, StoreObjectId.Parse(byteArray2, 0), true);
				}
				return;
			}
			ExTraceGlobals.UserConfigurationTracer.TraceError<int>((long)this.GetHashCode(), "UserConfigurationCache::Load. The retreived cache count is too big, persisted data is corrupted. Count = {0}.", num);
		}

		internal void SerializeUserConfigurationCacheEntries(BinaryWriter writer)
		{
			writer.Write(1);
			int capacity = 0;
			lock (this.userConfigCacheEntryList)
			{
				capacity = this.userConfigCacheEntryList.Count;
			}
			List<UserConfigurationCacheEntry> list = new List<UserConfigurationCacheEntry>(capacity);
			lock (this.userConfigCacheEntryList)
			{
				list.AddRange(new List<UserConfigurationCacheEntry>(this.userConfigCacheEntryList));
			}
			writer.Write(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				UserConfigurationCacheEntry userConfigurationCacheEntry = list[i];
				string configurationName = userConfigurationCacheEntry.ConfigurationName;
				writer.Write(configurationName);
				byte[] bytes = userConfigurationCacheEntry.FolderId.GetBytes();
				writer.Write(bytes.Length);
				writer.Write(bytes);
				byte[] bytes2 = userConfigurationCacheEntry.ItemId.GetBytes();
				writer.Write(bytes2.Length);
				writer.Write(bytes2);
			}
		}

		internal void AddUserConfigurationCachedEntry(StoreObjectId folderId, string configName, StoreObjectId itemId, bool canCleanse)
		{
			UserConfigurationCacheEntry value = new UserConfigurationCacheEntry(configName, folderId, itemId);
			LinkedListNode<UserConfigurationCacheEntry> node = new LinkedListNode<UserConfigurationCacheEntry>(value);
			lock (this.userConfigCacheEntryList)
			{
				for (LinkedListNode<UserConfigurationCacheEntry> linkedListNode = this.userConfigCacheEntryList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					UserConfigurationCacheEntry value2 = linkedListNode.Value;
					if (value2.CheckMatch(configName, folderId))
					{
						linkedListNode.Value = value;
						return;
					}
				}
				if (canCleanse && this.userConfigCacheEntryList.Count > 32)
				{
					this.userConfigCacheEntryList.RemoveLast();
					ExTraceGlobals.UserConfigurationTracer.TraceDebug<int>((long)this.GetHashCode(), "UserConfigurationCache::Add. The cache has reached the capacity: {0} the last entry was removed.", 32);
				}
				this.userConfigCacheEntryList.AddFirst(node);
			}
		}

		internal bool TryGetUserConfigurationCachedEntry(UserConfigurationName configName, StoreObjectId folderId, out UserConfigurationCacheEntry cacheEntry)
		{
			bool result;
			lock (this.userConfigCacheEntryList)
			{
				for (LinkedListNode<UserConfigurationCacheEntry> linkedListNode = this.userConfigCacheEntryList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					UserConfigurationCacheEntry value = linkedListNode.Value;
					if (value.CheckMatch(configName.Name, folderId))
					{
						cacheEntry = value;
						return true;
					}
				}
				cacheEntry = null;
				result = false;
			}
			return result;
		}

		private const int MaxCachedUserConfigurationCacheEntries = 32;

		private const ushort StreamBlobVersion = 1;

		private readonly LinkedList<UserConfigurationCacheEntry> userConfigCacheEntryList = new LinkedList<UserConfigurationCacheEntry>();

		private readonly DefaultFolderData[] defaultFolderData;

		private bool hasUserConfigurationLoaded;

		private bool defaultFoldersInitialized;
	}
}
