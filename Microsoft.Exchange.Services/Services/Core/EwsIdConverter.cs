using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class EwsIdConverter : BaseAlternateIdConverter
	{
		internal override IdFormat IdFormat
		{
			get
			{
				return IdFormat.EwsId;
			}
		}

		internal override CanonicalConvertedId Parse(AlternateId altId)
		{
			Util.ValidateSmtpAddress(altId.Mailbox);
			IdHeaderInformation idHeaderInformation = this.ConvertFromConcatenatedId(IdStorageType.MailboxItemMailboxGuidBased, altId.Id);
			StoreObjectId storeId = EwsIdConverter.ConvertIdHeaderToStoreObjectId(idHeaderInformation);
			ExchangePrincipal fromCache = ExchangePrincipalCache.GetFromCache(idHeaderInformation.MailboxId, CallContext.Current.ADRecipientSessionContext);
			return CanonicalConvertedId.CreateFromMailboxStoreId(storeId, fromCache.MailboxInfo.PrimarySmtpAddress.ToString(), altId.IsArchive);
		}

		internal override CanonicalConvertedId Parse(AlternatePublicFolderId altId)
		{
			IdHeaderInformation idHeaderInformation = this.ConvertFromConcatenatedId(IdStorageType.PublicFolder, altId.FolderId);
			StoreObjectId folderId = EwsIdConverter.ConvertBytesToStoreObjectId(idHeaderInformation.StoreIdBytes);
			return CanonicalConvertedId.CreateFromPublicFolderId(folderId);
		}

		internal override CanonicalConvertedId Parse(AlternatePublicFolderItemId altId)
		{
			IdHeaderInformation idHeaderInformation = this.ConvertFromConcatenatedId(IdStorageType.PublicFolderItem, altId.ItemId);
			StoreObjectId itemId = EwsIdConverter.ConvertIdHeaderToStoreObjectId(idHeaderInformation);
			StoreObjectId folderId = EwsIdConverter.ConvertBytesToStoreObjectId(idHeaderInformation.FolderIdBytes);
			return CanonicalConvertedId.CreateFromPublicFolderItemId(itemId, folderId);
		}

		internal override AlternateIdBase Format(CanonicalConvertedId canonicalId)
		{
			switch (canonicalId.ObjectType)
			{
			case IdStorageType.MailboxItemSmtpAddressBased:
				return this.FormatAlternateIdEmailAddress(canonicalId);
			case IdStorageType.PublicFolder:
				return this.FormatAlternatePublicFolderId(canonicalId);
			case IdStorageType.PublicFolderItem:
				return this.FormatAlternatePublcFolderItemId(canonicalId);
			default:
				return null;
			}
		}

		internal override string ConvertStoreObjectIdToString(StoreObjectId storeObjectId)
		{
			return Convert.ToBase64String(storeObjectId.ProviderLevelItemId);
		}

		private static StoreObjectId ConvertIdHeaderToStoreObjectId(IdHeaderInformation idHeaderInformation)
		{
			return idHeaderInformation.ToStoreObjectId();
		}

		private static StoreObjectId ConvertBytesToStoreObjectId(byte[] entryIdBytes)
		{
			StoreObjectId result;
			try
			{
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(entryIdBytes);
				result = storeObjectId;
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException);
			}
			return result;
		}

		private AlternateId FormatAlternateIdEmailAddress(CanonicalConvertedId canonicalId)
		{
			ExchangePrincipal fromCache = ExchangePrincipalCache.GetFromCache(canonicalId.PrimarySmtpAddress, CallContext.Current.ADRecipientSessionContext, canonicalId.IsArchive);
			AlternateId result;
			try
			{
				MailboxId mailboxId = new MailboxId(fromCache.MailboxInfo.MailboxGuid, canonicalId.IsArchive);
				string id = IdConverter.GetConcatenatedId(canonicalId.StoreObjectId, mailboxId, null).Id;
				result = new AlternateId(id, fromCache.MailboxInfo.PrimarySmtpAddress.ToString(), this.IdFormat, canonicalId.IsArchive);
			}
			catch (NonExistentMailboxGuidException ex)
			{
				throw new NonExistentMailboxException((CoreResources.IDs)3279543955U, ex.MailboxGuid.ToString());
			}
			return result;
		}

		private AlternatePublicFolderId FormatAlternatePublicFolderId(CanonicalConvertedId canonicalId)
		{
			canonicalId.StoreObjectId.UpdateItemType(StoreObjectType.Folder);
			string id = IdConverter.GetConcatenatedIdForPublicFolder(canonicalId.StoreObjectId).Id;
			return new AlternatePublicFolderId(id, this.IdFormat);
		}

		private AlternatePublicFolderItemId FormatAlternatePublcFolderItemId(CanonicalConvertedId canonicalId)
		{
			canonicalId.FolderId.UpdateItemType(StoreObjectType.Folder);
			string id = IdConverter.GetConcatenatedIdForPublicFolderItem(canonicalId.StoreObjectId, canonicalId.FolderId, null).Id;
			return new AlternatePublicFolderItemId(id, string.Empty, this.IdFormat);
		}

		private IdHeaderInformation ConvertFromConcatenatedId(IdStorageType expectedIdType, string encodedId)
		{
			if (string.IsNullOrEmpty(encodedId))
			{
				ExTraceGlobals.ConvertIdCallTracer.TraceDebug((long)this.GetHashCode(), "[EwsIdConverter::ConvertFromConcatenatedId] string encodedId passed in was either null or empty");
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U);
			}
			IdHeaderInformation idHeaderInformation = IdConverter.ConvertFromConcatenatedId(encodedId, BasicTypes.Item, null, true);
			if (expectedIdType != idHeaderInformation.IdStorageType)
			{
				throw new InvalidStoreIdException(ResponseCodeType.ErrorInvalidIdMalformed, (CoreResources.IDs)3010537222U);
			}
			return idHeaderInformation;
		}
	}
}
