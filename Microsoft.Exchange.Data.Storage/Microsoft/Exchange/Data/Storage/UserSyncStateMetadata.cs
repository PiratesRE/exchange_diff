using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserSyncStateMetadata
	{
		public UserSyncStateMetadata(MailboxSession mailboxSession)
		{
			this.Name = mailboxSession.MailboxOwner.MailboxInfo.DisplayName;
			this.MailboxGuid = mailboxSession.MailboxGuid;
		}

		public string Name { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public DeviceSyncStateMetadata TryRemove(DeviceIdentity deviceIdentity, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			DeviceSyncStateMetadata result;
			bool arg = this.devices.TryRemove(deviceIdentity, out result);
			syncLogger.TraceDebug<DeviceIdentity, bool>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.TryRemove] Tried to remove '{0}'.  Success? {1}", deviceIdentity, arg);
			return result;
		}

		public void Clear()
		{
			this.devices.Clear();
		}

		public DeviceSyncStateMetadata GetDevice(MailboxSession mailboxSession, DeviceIdentity deviceIdentity, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			DeviceSyncStateMetadata result;
			if (this.devices.TryGetValue(deviceIdentity, out result))
			{
				syncLogger.TraceDebug<DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetDevice] Cache hit for device: {0}", deviceIdentity);
				return result;
			}
			syncLogger.TraceDebug<DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetDevice] Cache MISS for device: {0}", deviceIdentity);
			DeviceSyncStateMetadata result2;
			using (Folder syncRootFolder = this.GetSyncRootFolder(mailboxSession, syncLogger))
			{
				using (QueryResult queryResult = syncRootFolder.FolderQuery(FolderQueryFlags.None, null, UserSyncStateMetadata.displayNameSort, new PropertyDefinition[]
				{
					FolderSchema.DisplayName,
					FolderSchema.Id
				}))
				{
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, deviceIdentity.CompositeKey)))
					{
						IStorePropertyBag storePropertyBag = queryResult.GetPropertyBags(1)[0];
						StoreObjectId objectId = ((VersionedId)storePropertyBag.TryGetProperty(FolderSchema.Id)).ObjectId;
						string text = (string)storePropertyBag.TryGetProperty(FolderSchema.DisplayName);
						DeviceSyncStateMetadata deviceSyncStateMetadata = new DeviceSyncStateMetadata(mailboxSession, objectId, syncLogger);
						DeviceSyncStateMetadata orAdd = this.devices.GetOrAdd(deviceIdentity, deviceSyncStateMetadata);
						if (!object.ReferenceEquals(deviceSyncStateMetadata, orAdd))
						{
							syncLogger.TraceDebug<DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetDevice] Race condition adding new device '{0}' to cache.  Disarding new instance.", deviceIdentity);
						}
						else
						{
							syncLogger.TraceDebug<DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetDevice] Added new device instance to user cache: {0}", deviceIdentity);
						}
						result2 = orAdd;
					}
					else
					{
						syncLogger.TraceDebug<SmtpAddress, DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetDevice] Mailbox '{0}' does not contain a device folder for '{1}'", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, deviceIdentity);
						result2 = null;
					}
				}
			}
			return result2;
		}

		public DeviceSyncStateMetadata GetOrAdd(DeviceSyncStateMetadata toAdd)
		{
			return this.devices.GetOrAdd(toAdd.Id, toAdd);
		}

		public List<DeviceSyncStateMetadata> GetAllDevices(MailboxSession mailboxSession, bool forceRefresh, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			syncLogger.TraceDebug<SmtpAddress, bool>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetAllDevices] Getting all devies for Mailbox: {0}, forceRefresh: {1}", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, forceRefresh);
			List<DeviceSyncStateMetadata> list = null;
			using (Folder syncRootFolder = this.GetSyncRootFolder(mailboxSession, syncLogger))
			{
				using (QueryResult queryResult = syncRootFolder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
				{
					FolderSchema.Id,
					FolderSchema.DisplayName
				}))
				{
					for (;;)
					{
						object[][] rows = queryResult.GetRows(100);
						if (rows == null || rows.Length == 0)
						{
							break;
						}
						object[][] array = rows;
						for (int i = 0; i < array.Length; i++)
						{
							object[] array2 = array[i];
							StoreObjectId deviceFolderId = ((VersionedId)array2[0]).ObjectId;
							DeviceIdentity deviceIdentity = new DeviceIdentity((string)array2[1]);
							syncLogger.TraceDebug<DeviceIdentity>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetAllDevices] Found device: {0}", deviceIdentity);
							DeviceSyncStateMetadata item;
							if (forceRefresh)
							{
								item = this.devices.AddOrUpdate(deviceIdentity, new DeviceSyncStateMetadata(mailboxSession, deviceFolderId, syncLogger), (DeviceIdentity key, DeviceSyncStateMetadata old) => new DeviceSyncStateMetadata(mailboxSession, deviceFolderId, syncLogger));
							}
							else
							{
								item = this.devices.GetOrAdd(deviceIdentity, (DeviceIdentity key) => new DeviceSyncStateMetadata(mailboxSession, deviceFolderId, syncLogger));
							}
							if (list == null)
							{
								list = new List<DeviceSyncStateMetadata>();
							}
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		public override string ToString()
		{
			return string.Format("User: {0} - {1}", this.Name, this.MailboxGuid);
		}

		private Folder GetSyncRootFolder(MailboxSession mailboxSession, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			Folder result = null;
			try
			{
				result = Folder.Bind(mailboxSession, DefaultFolderType.SyncRoot);
			}
			catch (ObjectNotFoundException)
			{
				syncLogger.TraceDebug<SmtpAddress>(ExTraceGlobals.SyncTracer, (long)this.GetHashCode(), "[UserSyncStateMetadata.GetDevice] Missing SyncRoot folder for mailbox {0}.  Recreating.", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress);
				mailboxSession.CreateDefaultFolder(DefaultFolderType.SyncRoot);
				result = Folder.Bind(mailboxSession, DefaultFolderType.SyncRoot);
			}
			return result;
		}

		private static readonly SortBy[] displayNameSort = new SortBy[]
		{
			new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
		};

		private ConcurrentDictionary<DeviceIdentity, DeviceSyncStateMetadata> devices = new ConcurrentDictionary<DeviceIdentity, DeviceSyncStateMetadata>();
	}
}
