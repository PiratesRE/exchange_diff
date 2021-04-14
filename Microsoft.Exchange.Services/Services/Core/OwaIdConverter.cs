using System;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class OwaIdConverter : BaseAlternateIdConverter
	{
		internal override CanonicalConvertedId Parse(AlternateId altId)
		{
			Util.ValidateSmtpAddress(altId.Mailbox);
			CanonicalConvertedId canonicalConvertedId = this.ParseOwaString(IdStorageType.MailboxItemSmtpAddressBased, altId.Id);
			canonicalConvertedId.PrimarySmtpAddress = altId.Mailbox;
			canonicalConvertedId.IsArchive = altId.IsArchive;
			return canonicalConvertedId;
		}

		internal override CanonicalConvertedId Parse(AlternatePublicFolderId altId)
		{
			return this.ParseOwaString(IdStorageType.PublicFolder, altId.FolderId);
		}

		internal override CanonicalConvertedId Parse(AlternatePublicFolderItemId altId)
		{
			return this.ParseOwaString(IdStorageType.PublicFolderItem, altId.ItemId);
		}

		internal override AlternateIdBase Format(CanonicalConvertedId canonicalId)
		{
			switch (canonicalId.ObjectType)
			{
			case IdStorageType.MailboxItemSmtpAddressBased:
				return new AlternateId(this.ConvertStoreObjectIdToString(canonicalId.StoreObjectId), canonicalId.PrimarySmtpAddress, this.IdFormat, canonicalId.IsArchive);
			case IdStorageType.PublicFolder:
				return this.FormatPublicFolderId(canonicalId);
			case IdStorageType.PublicFolderItem:
				return this.FormatPublicFolderItemId(canonicalId);
			default:
				return null;
			}
		}

		internal override string ConvertStoreObjectIdToString(StoreObjectId storeObjectId)
		{
			string value = storeObjectId.ToBase64String();
			return WebUtility.UrlEncode(value);
		}

		internal override IdFormat IdFormat
		{
			get
			{
				return IdFormat.OwaId;
			}
		}

		internal CanonicalConvertedId ParseOwaString(IdStorageType expectedIdType, string owaString)
		{
			owaString = WebUtility.UrlDecode(owaString);
			IdStorageType idStorageType = OwaIdConverter.DetermineIdTypeFromOwaString(owaString);
			if (idStorageType != expectedIdType)
			{
				throw new InvalidStoreIdException(ResponseCodeType.ErrorInvalidIdMalformed, (CoreResources.IDs)3010537222U);
			}
			switch (idStorageType)
			{
			case IdStorageType.MailboxItemSmtpAddressBased:
			{
				bool flag = OwaIdConverter.IsArchiveIdFromOwaString(owaString);
				if (flag)
				{
					owaString = owaString.Substring("AMB".Length + 1);
				}
				StoreObjectId storeId = this.ConvertStringToStoreObjectId(owaString);
				return CanonicalConvertedId.CreateFromMailboxStoreId(storeId, string.Empty, flag);
			}
			case IdStorageType.PublicFolder:
				return this.ParsePublicFolderIdString(owaString);
			case IdStorageType.PublicFolderItem:
				return this.ParsePublicFolderItemIdString(owaString);
			default:
				return null;
			}
		}

		private static IdStorageType DetermineIdTypeFromOwaString(string owaString)
		{
			if (owaString.StartsWith("PSF."))
			{
				return IdStorageType.PublicFolder;
			}
			if (owaString.StartsWith("PSI."))
			{
				return IdStorageType.PublicFolderItem;
			}
			return IdStorageType.MailboxItemSmtpAddressBased;
		}

		private static bool IsArchiveIdFromOwaString(string owaString)
		{
			return owaString.StartsWith("AMB.");
		}

		private CanonicalConvertedId ParsePublicFolderItemIdString(string owaString)
		{
			int num = owaString.LastIndexOf(".");
			if (num == "PSI".Length)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U);
			}
			string idValue = owaString.Substring("PSI".Length + 1, num - "PSI".Length - 1);
			string idValue2 = owaString.Substring(num + 1);
			StoreObjectId folderId = this.ConvertStringToStoreObjectId(idValue);
			StoreObjectId itemId = this.ConvertStringToStoreObjectId(idValue2);
			return CanonicalConvertedId.CreateFromPublicFolderItemId(itemId, folderId);
		}

		private CanonicalConvertedId ParsePublicFolderIdString(string owaString)
		{
			string idValue = owaString.Substring("PSF".Length + 1);
			StoreObjectId folderId = this.ConvertStringToStoreObjectId(idValue);
			return CanonicalConvertedId.CreateFromPublicFolderId(folderId);
		}

		private StoreObjectId ConvertStringToStoreObjectId(string idValue)
		{
			if (string.IsNullOrEmpty(idValue))
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U);
			}
			StoreObjectId result;
			try
			{
				result = StoreObjectId.Deserialize(idValue);
			}
			catch (FormatException innerException)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException2);
			}
			return result;
		}

		private AlternatePublicFolderId FormatPublicFolderId(CanonicalConvertedId id)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PSF");
			stringBuilder.Append(".");
			stringBuilder.Append(this.ConvertStoreObjectIdToString(id.StoreObjectId));
			return new AlternatePublicFolderId(stringBuilder.ToString(), this.IdFormat);
		}

		private AlternatePublicFolderItemId FormatPublicFolderItemId(CanonicalConvertedId id)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PSI");
			stringBuilder.Append(".");
			stringBuilder.Append(this.ConvertStoreObjectIdToString(id.FolderId));
			stringBuilder.Append(".");
			stringBuilder.Append(this.ConvertStoreObjectIdToString(id.StoreObjectId));
			return new AlternatePublicFolderItemId(stringBuilder.ToString(), string.Empty, this.IdFormat);
		}

		private const string PublicFolderFolderPrefix = "PSF";

		private const string PublicFolderItemPrefix = "PSI";

		private const string ArchiveMailBoxObjectPrefix = "AMB";

		private const string SeparatorChar = ".";
	}
}
