using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UserConfigurationCache
	{
		internal UserConfigurationCache(MailboxSession store)
		{
			this.mailboxSession = store;
		}

		private MailboxSessionSharableDataManager SharedCacheEntryManager
		{
			get
			{
				if (!this.mailboxSession.SharedDataManager.HasCacheEntryLoaded)
				{
					this.Load();
				}
				return this.mailboxSession.SharedDataManager;
			}
		}

		internal void Add(StoreObjectId folderId, UserConfigurationName configName, StoreObjectId itemId)
		{
			this.SharedCacheEntryManager.AddUserConfigurationCachedEntry(folderId, configName.Name, itemId, this.mailboxSession.LogonType == LogonType.Owner);
		}

		internal void Remove(UserConfigurationName configName, StoreObjectId folderId)
		{
			this.SharedCacheEntryManager.Remove(configName, folderId);
		}

		internal UserConfiguration Get(UserConfigurationName configName, StoreObjectId folderId)
		{
			bool flag = false;
			UserConfigurationCacheEntry userConfigurationCacheEntry;
			if (!this.SharedCacheEntryManager.TryGetUserConfigurationCachedEntry(configName, folderId, out userConfigurationCacheEntry))
			{
				ExTraceGlobals.UserConfigurationTracer.TraceDebug<UserConfigurationName, StoreObjectId>((long)this.GetHashCode(), "UserConfigurationCache::Get. Miss the cache. ConfigName = {0}, folderId = {1}.", configName, folderId);
				return null;
			}
			StoreObjectId itemId = userConfigurationCacheEntry.ItemId;
			ConfigurationItem configurationItem = null;
			bool flag2 = false;
			UserConfiguration result;
			try
			{
				configurationItem = ConfigurationItem.Bind(this.mailboxSession, itemId);
				ExTraceGlobals.UserConfigurationTracer.TraceDebug<UserConfigurationName>((long)this.GetHashCode(), "UserConfigurationCache::Get. Hit the item in the cache. ConfigName = {0}.", configName);
				object obj = configurationItem.TryGetProperty(InternalSchema.UserConfigurationType);
				if (!(obj is int))
				{
					ExTraceGlobals.UserConfigurationTracer.TraceDebug<UserConfigurationName, object>((long)this.GetHashCode(), "UserConfigurationCache::Get. The type has been corrupted. ConfigName = {0}. Actual = {1}.", configName, configurationItem.TryGetProperty(InternalSchema.UserConfigurationType));
					ExTraceGlobals.UserConfigurationTracer.TraceError((long)this.GetHashCode(), "UserConfiguration::Get. The configuration type has been corrupted. PropValue = {0}.", new object[]
					{
						configurationItem.TryGetProperty(InternalSchema.UserConfigurationType)
					});
					throw new CorruptDataException(ServerStrings.ExConfigDataCorrupted("ConfigurationType"));
				}
				UserConfigurationTypes type = UserConfiguration.CheckUserConfigurationType((int)obj);
				ExTraceGlobals.UserConfigurationTracer.TraceDebug<UserConfigurationName>((long)this.GetHashCode(), "UserConfigurationCache::Get. Hit the cache. ConfigName = {0}.", configName);
				UserConfiguration userConfiguration = new UserConfiguration(configurationItem, folderId, configName, type, true);
				flag2 = true;
				result = userConfiguration;
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.UserConfigurationTracer.TraceDebug<UserConfigurationName>((long)this.GetHashCode(), "UserConfigurationCache::Get. Miss the cache. The object has been deleted. ConfigName = {0}.", configName);
				ExTraceGlobals.UserConfigurationTracer.TraceDebug<UserConfigurationName>((long)this.GetHashCode(), "The user configuration object cannot be found though we hold its itemId in cache. ConfigName = {0}.", configName);
				this.Remove(configName, folderId);
				flag = true;
				result = null;
			}
			finally
			{
				if (!flag2)
				{
					if (configurationItem != null)
					{
						configurationItem.Dispose();
					}
					if (flag && this.mailboxSession.LogonType == LogonType.Owner)
					{
						this.Save();
					}
				}
			}
			return result;
		}

		internal void Save()
		{
			if (!this.SharedCacheEntryManager.HasCacheEntries)
			{
				if (!this.SharedCacheEntryManager.HasCacheEntryLoaded)
				{
					return;
				}
			}
			try
			{
				using (Folder folder = Folder.Bind(this.mailboxSession, DefaultFolderType.Inbox, new PropertyDefinition[]
				{
					InternalSchema.OwnerLogonUserConfigurationCache
				}))
				{
					using (MemoryStream memoryStream = new MemoryStream(Math.Max(2048, this.cacheSizeOnLoad)))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
						{
							this.SharedCacheEntryManager.SerializeUserConfigurationCacheEntries(binaryWriter);
							folder[InternalSchema.OwnerLogonUserConfigurationCache] = memoryStream.ToArray();
							folder.Save();
						}
					}
				}
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<ArgumentException>(0L, "UserConfigurationCache::Save. The stream is invalid. Exception = {0}.", arg);
			}
		}

		private void Load()
		{
			if (this.mailboxSession.LogonType == LogonType.Owner)
			{
				try
				{
					byte[] array;
					using (Folder folder = Folder.Bind(this.mailboxSession, DefaultFolderType.Inbox, new PropertyDefinition[]
					{
						InternalSchema.OwnerLogonUserConfigurationCache
					}))
					{
						array = (folder.TryGetProperty(InternalSchema.OwnerLogonUserConfigurationCache) as byte[]);
					}
					if (array != null)
					{
						this.cacheSizeOnLoad = array.Length;
						using (MemoryStream memoryStream = new MemoryStream(array))
						{
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								this.mailboxSession.SharedDataManager.ParseUserConfigurationCacheEntries(binaryReader);
							}
						}
					}
				}
				catch (EndOfStreamException arg)
				{
					ExTraceGlobals.StorageTracer.TraceError<EndOfStreamException>(0L, "UserConfigurationCache::Load. The end of stream was reached.  Exception = {0}.", arg);
				}
				catch (ArgumentException arg2)
				{
					ExTraceGlobals.StorageTracer.TraceError<ArgumentException>(0L, "UserConfigurationCache::Load. The cache data is corrupted and cannot be deserialized. Exception = {0}.", arg2);
				}
				catch (CorruptDataException arg3)
				{
					ExTraceGlobals.StorageTracer.TraceError<CorruptDataException>(0L, "UserConfigurationCache::Load. The cache data is corrupted and cannot be deserialized. Exception = {0}.", arg3);
				}
			}
		}

		private const int DefaultCacheSize = 2048;

		private const int MaxCachedConfigs = 32;

		private const ushort StreamBlobVersion = 1;

		private int cacheSizeOnLoad;

		private MailboxSession mailboxSession;
	}
}
