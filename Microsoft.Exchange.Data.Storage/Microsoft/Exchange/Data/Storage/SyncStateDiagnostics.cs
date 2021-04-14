using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SyncStateDiagnostics
	{
		public static GetSyncStateResult GetData(MailboxSession session, ParsedCallData callData)
		{
			GetSyncStateResult getSyncStateResult = new GetSyncStateResult();
			getSyncStateResult.LoggingEnabled = SyncStateStorage.GetMailboxLoggingEnabled(session, null);
			using (SyncStateStorage.GetSyncFolderRoot(session, null))
			{
				UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(session, null);
				List<DeviceSyncStateMetadata> allDevices = userSyncStateMetadata.GetAllDevices(session, true, null);
				getSyncStateResult.Devices = new List<DeviceData>(allDevices.Count);
				foreach (DeviceSyncStateMetadata deviceSyncStateMetadata in allDevices)
				{
					if (SyncStateDiagnostics.ShouldAddDevice(callData, deviceSyncStateMetadata.Id))
					{
						DeviceData deviceData = new DeviceData
						{
							Name = deviceSyncStateMetadata.Id.CompositeKey,
							SyncFolders = new List<SyncStateFolderData>(),
							FolderId = deviceSyncStateMetadata.DeviceFolderId
						};
						getSyncStateResult.Devices.Add(deviceData);
						foreach (KeyValuePair<string, SyncStateMetadata> keyValuePair in deviceSyncStateMetadata.SyncStates)
						{
							bool flag = string.Equals(keyValuePair.Key, callData.SyncStateName, StringComparison.OrdinalIgnoreCase);
							if (callData.SyncStateName == null || flag)
							{
								SyncStateFolderData syncStateFolderData = new SyncStateFolderData
								{
									Name = keyValuePair.Key,
									StorageType = keyValuePair.Value.StorageType.ToString()
								};
								if (flag)
								{
									SyncStateDiagnostics.GetSyncStateBlob(session, keyValuePair.Value, syncStateFolderData);
								}
								else
								{
									syncStateFolderData.SyncStateSize = -1;
								}
								deviceData.SyncFolders.Add(syncStateFolderData);
							}
						}
					}
				}
			}
			return getSyncStateResult;
		}

		private static void GetSyncStateBlob(MailboxSession session, SyncStateMetadata syncStateMetadata, SyncStateFolderData data)
		{
			switch (syncStateMetadata.StorageType)
			{
			case StorageType.Folder:
				using (Folder folder = Folder.Bind(session, syncStateMetadata.FolderSyncStateId, new PropertyDefinition[]
				{
					ItemSchema.SyncCustomState
				}))
				{
					object obj = folder.TryGetProperty(ItemSchema.SyncCustomState);
					byte[] array = obj as byte[];
					data.SyncStateBlob = Convert.ToBase64String(array);
					data.SyncStateSize = array.Length;
					data.Created = (DateTime)folder.CreationTime;
					return;
				}
				break;
			case StorageType.Item:
			case StorageType.DirectItem:
				break;
			default:
				return;
			}
			using (Item item = Item.Bind(session, syncStateMetadata.ItemSyncStateId, new PropertyDefinition[]
			{
				ItemSchema.SyncCustomState
			}))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (Stream stream = item.OpenPropertyStream(ItemSchema.SyncCustomState, PropertyOpenMode.ReadOnly))
					{
						byte[] data2 = new byte[1024];
						Util.StreamHandler.CopyStreamData(stream, memoryStream, null, 0, data2);
						memoryStream.Flush();
					}
					data.Created = (DateTime)item.CreationTime;
					data.SyncStateSize = (int)memoryStream.Position;
					string syncStateBlob = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Position, Base64FormattingOptions.None);
					data.SyncStateBlob = syncStateBlob;
				}
			}
		}

		private static bool ShouldAddDevice(ParsedCallData callData, DeviceIdentity deviceIdentity)
		{
			return callData.DeviceId == null || deviceIdentity.Equals(callData.DeviceId, callData.DeviceType);
		}

		private const int DisplayNameIndex = 0;

		private const int CreationTimeIndex = 1;

		private const int ParentItemIdIndex = 2;

		private const int ItemCountIndex = 3;

		private const int LastModifiedTimeIndex = 4;

		private const int IdIndex = 5;

		private static readonly StorePropertyDefinition[] FolderQueryProps = new StorePropertyDefinition[]
		{
			FolderSchema.DisplayName,
			StoreObjectSchema.CreationTime,
			StoreObjectSchema.ParentItemId,
			FolderSchema.ItemCount,
			StoreObjectSchema.LastModifiedTime,
			FolderSchema.Id
		};
	}
}
