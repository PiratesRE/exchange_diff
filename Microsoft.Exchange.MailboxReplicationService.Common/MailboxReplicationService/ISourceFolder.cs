using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface ISourceFolder : IFolder, IDisposable
	{
		void CopyTo(IFxProxy destFolder, CopyPropertiesFlags flags, PropTag[] excludeTags);

		void ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds);

		FolderChangesManifest EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges);

		List<MessageRec> EnumerateMessagesPaged(int maxPageSize);

		int GetEstimatedItemCount();
	}
}
