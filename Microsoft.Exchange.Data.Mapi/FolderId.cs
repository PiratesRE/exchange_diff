using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public abstract class FolderId : MapiObjectId
	{
		public string LegacyDistinguishedName
		{
			get
			{
				return this.legacyDistinguishedName;
			}
		}

		public MessageStoreId MessageStoreId
		{
			get
			{
				return this.messageStoreId;
			}
		}

		public MapiFolderPath MapiFolderPath
		{
			get
			{
				return this.mapiFolderPath;
			}
		}

		public FolderId()
		{
		}

		public FolderId(byte[] bytes) : base(bytes)
		{
		}

		public FolderId(MessageStoreId storeId, string legacyDn)
		{
			this.messageStoreId = storeId;
			this.legacyDistinguishedName = legacyDn;
		}

		public FolderId(MessageStoreId storeId, MapiEntryId entryId) : base(entryId)
		{
			this.messageStoreId = storeId;
		}

		public FolderId(MessageStoreId storeId, MapiFolderPath folderPath)
		{
			this.messageStoreId = storeId;
			this.mapiFolderPath = folderPath;
		}

		internal FolderId(MessageStoreId storeId, MapiEntryId entryId, MapiFolderPath folderPath, string legacyDn) : base(entryId)
		{
			this.messageStoreId = storeId;
			this.mapiFolderPath = folderPath;
			this.legacyDistinguishedName = legacyDn;
		}

		private readonly string legacyDistinguishedName;

		private readonly MessageStoreId messageStoreId;

		private readonly MapiFolderPath mapiFolderPath;
	}
}
