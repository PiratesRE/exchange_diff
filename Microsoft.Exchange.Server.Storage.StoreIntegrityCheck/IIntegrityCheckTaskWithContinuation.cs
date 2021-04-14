using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IIntegrityCheckTaskWithContinuation
	{
		bool ContinueExecuteOnFolder(Context context, MailboxEntry mailboxEntry, FolderEntry folderEntry);
	}
}
