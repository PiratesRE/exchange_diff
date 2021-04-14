using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FolderManifestEntry : ICustomSerializable
	{
		public FolderManifestEntry()
		{
		}

		public FolderManifestEntry(StoreObjectId itemId)
		{
			this.internalItemId = itemId;
			this.changeTrackingHash = -1;
		}

		public StoreObjectId ItemId
		{
			get
			{
				return this.internalItemId;
			}
		}

		public byte[] ChangeKey
		{
			get
			{
				return this.changeKey;
			}
			set
			{
				this.changeKey = value;
			}
		}

		public int ChangeTrackingHash
		{
			get
			{
				return this.changeTrackingHash;
			}
			set
			{
				this.changeTrackingHash = value;
			}
		}

		public ChangeType ChangeType
		{
			get
			{
				return this.changeType;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<ChangeType>(value, "value");
				this.changeType = value;
			}
		}

		public StoreObjectId ParentId
		{
			get
			{
				return this.parentId;
			}
			set
			{
				this.parentId = value;
			}
		}

		public SyncPermissions Permissions
		{
			get
			{
				return this.permissions;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<SyncPermissions>(value, "value");
				this.permissions = value;
			}
		}

		public string Owner { get; set; }

		public bool Hidden { get; set; }

		public string DisplayName { get; set; }

		public string ClassName { get; set; }

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			StoreObjectIdData storeObjectIdDataInstance = componentDataPool.GetStoreObjectIdDataInstance();
			storeObjectIdDataInstance.DeserializeData(reader, componentDataPool);
			this.internalItemId = storeObjectIdDataInstance.Data;
			storeObjectIdDataInstance.DeserializeData(reader, componentDataPool);
			this.parentId = storeObjectIdDataInstance.Data;
			this.changeType = (ChangeType)reader.ReadInt32();
			ByteArrayData byteArrayInstance = componentDataPool.GetByteArrayInstance();
			byteArrayInstance.DeserializeData(reader, componentDataPool);
			this.changeKey = byteArrayInstance.Data;
			this.changeTrackingHash = reader.ReadInt32();
			if (componentDataPool.InternalVersion > 1)
			{
				Int32Data int32DataInstance = componentDataPool.GetInt32DataInstance();
				int32DataInstance.DeserializeData(reader, componentDataPool);
				this.Permissions = (SyncPermissions)int32DataInstance.Data;
				StringData stringDataInstance = componentDataPool.GetStringDataInstance();
				stringDataInstance.DeserializeData(reader, componentDataPool);
				this.Owner = stringDataInstance.Data;
				BooleanData booleanDataInstance = componentDataPool.GetBooleanDataInstance();
				booleanDataInstance.DeserializeData(reader, componentDataPool);
				this.Hidden = booleanDataInstance.Data;
			}
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetStoreObjectIdDataInstance().Bind(this.internalItemId).SerializeData(writer, componentDataPool);
			componentDataPool.GetStoreObjectIdDataInstance().Bind(this.parentId).SerializeData(writer, componentDataPool);
			writer.Write((int)this.changeType);
			componentDataPool.GetByteArrayInstance().Bind(this.changeKey).SerializeData(writer, componentDataPool);
			writer.Write(this.changeTrackingHash);
			componentDataPool.GetInt32DataInstance().Bind((int)this.Permissions).SerializeData(writer, componentDataPool);
			componentDataPool.GetStringDataInstance().Bind(this.Owner).SerializeData(writer, componentDataPool);
			componentDataPool.GetBooleanDataInstance().Bind(this.Hidden).SerializeData(writer, componentDataPool);
		}

		private byte[] changeKey;

		private int changeTrackingHash;

		private ChangeType changeType;

		private StoreObjectId parentId;

		private StoreObjectId internalItemId;

		private SyncPermissions permissions;
	}
}
