using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FolderStateEntry : ICustomSerializableBuilder, ICustomSerializable
	{
		public FolderStateEntry()
		{
		}

		public FolderStateEntry(StoreObjectId parentId, byte[] changeKey, int changeTrackingHash)
		{
			this.parentId = parentId;
			this.changeKey = changeKey;
			this.changeTrackingHash = changeTrackingHash;
		}

		public ushort TypeId
		{
			get
			{
				return FolderStateEntry.typeId;
			}
			set
			{
				FolderStateEntry.typeId = value;
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

		public static explicit operator FolderStateEntry(FolderManifestEntry manifestEntry)
		{
			return new FolderStateEntry(manifestEntry.ParentId, manifestEntry.ChangeKey, manifestEntry.ChangeTrackingHash);
		}

		public ICustomSerializable BuildObject()
		{
			return new FolderStateEntry();
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			StoreObjectIdData storeObjectIdDataInstance = componentDataPool.GetStoreObjectIdDataInstance();
			storeObjectIdDataInstance.DeserializeData(reader, componentDataPool);
			this.parentId = storeObjectIdDataInstance.Data;
			ByteArrayData byteArrayInstance = componentDataPool.GetByteArrayInstance();
			byteArrayInstance.DeserializeData(reader, componentDataPool);
			this.changeKey = byteArrayInstance.Data;
			this.changeTrackingHash = reader.ReadInt32();
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetStoreObjectIdDataInstance().Bind(this.parentId).SerializeData(writer, componentDataPool);
			componentDataPool.GetByteArrayInstance().Bind(this.changeKey).SerializeData(writer, componentDataPool);
			writer.Write(this.changeTrackingHash);
		}

		private static ushort typeId;

		private byte[] changeKey;

		private int changeTrackingHash;

		private StoreObjectId parentId;
	}
}
