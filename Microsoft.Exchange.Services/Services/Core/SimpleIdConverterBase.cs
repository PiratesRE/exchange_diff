using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class SimpleIdConverterBase : BaseAlternateIdConverter
	{
		internal override CanonicalConvertedId Parse(AlternateId altId)
		{
			Util.ValidateSmtpAddress(altId.Mailbox);
			StoreObjectId storeId = this.ConvertStringToStoreObjectId(altId.Id);
			return CanonicalConvertedId.CreateFromMailboxStoreId(storeId, altId.Mailbox, altId.IsArchive);
		}

		internal override CanonicalConvertedId Parse(AlternatePublicFolderId altId)
		{
			StoreObjectId folderId = this.ConvertStringToStoreObjectId(altId.FolderId);
			return CanonicalConvertedId.CreateFromPublicFolderId(folderId);
		}

		internal override CanonicalConvertedId Parse(AlternatePublicFolderItemId altId)
		{
			StoreObjectId itemId = this.ConvertStringToStoreObjectId(altId.ItemId);
			StoreObjectId folderId = this.ConvertStringToStoreObjectId(altId.FolderId);
			return CanonicalConvertedId.CreateFromPublicFolderItemId(itemId, folderId);
		}

		internal override AlternateIdBase Format(CanonicalConvertedId canonicalId)
		{
			switch (canonicalId.ObjectType)
			{
			case IdStorageType.MailboxItemSmtpAddressBased:
				return new AlternateId(this.ConvertStoreObjectIdToString(canonicalId.StoreObjectId), canonicalId.PrimarySmtpAddress, this.IdFormat, canonicalId.IsArchive);
			case IdStorageType.PublicFolder:
				return new AlternatePublicFolderId(this.ConvertStoreObjectIdToString(canonicalId.StoreObjectId), this.IdFormat);
			case IdStorageType.PublicFolderItem:
				return new AlternatePublicFolderItemId(this.ConvertStoreObjectIdToString(canonicalId.StoreObjectId), this.ConvertStoreObjectIdToString(canonicalId.FolderId), this.IdFormat);
			default:
				return null;
			}
		}

		internal abstract StoreObjectId ConvertStringToStoreObjectId(string idValue);
	}
}
