using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core
{
	internal class CanonicalConvertedId
	{
		private CanonicalConvertedId(IdStorageType objectType, StoreObjectId storeObjectId, StoreObjectId folderId, string primarySmtpAddress, bool isArchive)
		{
			this.objectType = objectType;
			this.storeObjectId = storeObjectId;
			this.folderId = folderId;
			this.primarySmtpAddress = primarySmtpAddress;
			this.isArchive = isArchive;
		}

		internal static CanonicalConvertedId CreateFromPublicFolderId(StoreObjectId folderId)
		{
			return new CanonicalConvertedId(IdStorageType.PublicFolder, folderId, null, null, false);
		}

		internal static CanonicalConvertedId CreateFromPublicFolderItemId(StoreObjectId itemId, StoreObjectId folderId)
		{
			return new CanonicalConvertedId(IdStorageType.PublicFolderItem, itemId, folderId, null, false);
		}

		internal static CanonicalConvertedId CreateFromMailboxStoreId(StoreObjectId storeId, string primarySmtpAddress, bool isArchive)
		{
			return new CanonicalConvertedId(IdStorageType.MailboxItemSmtpAddressBased, storeId, null, primarySmtpAddress, isArchive);
		}

		internal IdStorageType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		internal StoreObjectId StoreObjectId
		{
			get
			{
				return this.storeObjectId;
			}
		}

		internal StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		internal string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddress;
			}
			set
			{
				this.primarySmtpAddress = value;
			}
		}

		internal bool IsArchive
		{
			get
			{
				return this.isArchive;
			}
			set
			{
				this.isArchive = value;
			}
		}

		private IdStorageType objectType;

		private StoreObjectId storeObjectId;

		private StoreObjectId folderId;

		private string primarySmtpAddress;

		private bool isArchive;
	}
}
