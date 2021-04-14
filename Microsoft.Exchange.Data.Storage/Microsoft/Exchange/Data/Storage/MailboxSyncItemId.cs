using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSyncItemId : ISyncItemId, ICustomSerializableBuilder, ICustomSerializable
	{
		public MailboxSyncItemId()
		{
		}

		protected MailboxSyncItemId(StoreObjectId id)
		{
			this.nativeId = id;
		}

		public object NativeId
		{
			get
			{
				return this.nativeId;
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

		public virtual ushort TypeId
		{
			get
			{
				return MailboxSyncItemId.typeId;
			}
			set
			{
				MailboxSyncItemId.typeId = value;
			}
		}

		public static MailboxSyncItemId CreateForExistingItem(FolderSync folderSync, StoreObjectId id)
		{
			MailboxSyncItemId mailboxSyncItemId = new MailboxSyncItemId(id);
			if (folderSync == null || folderSync.ClientState.ContainsKey(mailboxSyncItemId))
			{
				return mailboxSyncItemId;
			}
			return null;
		}

		public static MailboxSyncItemId CreateForNewItem(StoreObjectId id)
		{
			return new MailboxSyncItemId(id);
		}

		public virtual ICustomSerializable BuildObject()
		{
			return new MailboxSyncItemId();
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			StoreObjectIdData storeObjectIdDataInstance = componentDataPool.GetStoreObjectIdDataInstance();
			storeObjectIdDataInstance.DeserializeData(reader, componentDataPool);
			this.nativeId = storeObjectIdDataInstance.Data;
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetStoreObjectIdDataInstance().Bind(this.nativeId).SerializeData(writer, componentDataPool);
		}

		public override bool Equals(object syncItemId)
		{
			MailboxSyncItemId mailboxSyncItemId = syncItemId as MailboxSyncItemId;
			return mailboxSyncItemId != null && this.nativeId.Equals(mailboxSyncItemId.nativeId);
		}

		public override int GetHashCode()
		{
			return this.nativeId.GetHashCode();
		}

		public override string ToString()
		{
			if (this.nativeId != null)
			{
				return this.nativeId.ToString();
			}
			return "MailboxSyncItemId with null native id";
		}

		private static ushort typeId;

		private StoreObjectId nativeId;

		private byte[] changeKey;
	}
}
