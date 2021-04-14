using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal static class FolderIdHelper
	{
		internal static string GetIndexForFolderEntryId(byte[] folderEntryId)
		{
			return HexConverter.ByteArrayToHexString(folderEntryId, 22, 24);
		}

		internal static bool IsValidFolderEntryId(byte[] folderEntryId)
		{
			return folderEntryId != null && folderEntryId.Length == 46;
		}

		internal static StoreObjectId GetStoreObjectIdFromHexString(string folderId, MailboxSession mailboxSession)
		{
			byte[] longTermId = HexConverter.HexStringToByteArray(folderId);
			Array.Resize<byte>(ref longTermId, 22);
			long idFromLongTermId = mailboxSession.IdConverter.GetIdFromLongTermId(longTermId);
			return mailboxSession.IdConverter.CreateFolderId(idFromLongTermId);
		}
	}
}
