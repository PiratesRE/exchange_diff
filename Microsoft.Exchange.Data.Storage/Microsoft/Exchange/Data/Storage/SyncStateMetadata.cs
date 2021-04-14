using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncStateMetadata
	{
		public SyncStateMetadata(DeviceSyncStateMetadata parentDevice, string name, StoreObjectId folderSyncStateId, StoreObjectId itemSyncStateId)
		{
			ArgumentValidator.ThrowIfNull("parentDevice", parentDevice);
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			this.ParentDevice = parentDevice;
			this.Name = name;
			this.FolderSyncStateId = folderSyncStateId;
			this.ItemSyncStateId = itemSyncStateId;
			this.ParentDevice.TryAdd(this, null);
		}

		public DeviceSyncStateMetadata ParentDevice { get; private set; }

		public string Name { get; private set; }

		public StoreObjectId FolderSyncStateId { get; set; }

		public StoreObjectId ItemSyncStateId { get; set; }

		public StorageType StorageType
		{
			get
			{
				if (this.ItemSyncStateId == null)
				{
					return StorageType.Folder;
				}
				if (this.FolderSyncStateId != null)
				{
					return StorageType.Item;
				}
				return StorageType.DirectItem;
			}
		}

		public override string ToString()
		{
			StoreObjectId storeObjectId = (this.StorageType == StorageType.Folder) ? this.FolderSyncStateId : this.ItemSyncStateId;
			string arg = (storeObjectId == null) ? "NULL" : storeObjectId.ToBase64String();
			return string.Format("{0}[{1}]- {2}", this.Name, this.StorageType, arg);
		}
	}
}
