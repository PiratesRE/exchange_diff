using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IFxProxyPool : IDisposable
	{
		IFolderProxy CreateFolder(FolderRec folder);

		IFolderProxy GetFolderProxy(byte[] folderId);

		EntryIdMap<byte[]> GetFolderData();

		void Flush();

		List<byte[]> GetUploadedMessageIDs();

		void SetItemProperties(ItemPropertiesBase props);
	}
}
