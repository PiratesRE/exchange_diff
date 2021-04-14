using System;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	internal interface IHierarchySyncExecutor
	{
		void SyncSingleFolder(byte[] folderId);

		bool ProcessNextBatch();

		void EnsureDestinationFolderHasParentChain(FolderRec sourceFolderRec);
	}
}
