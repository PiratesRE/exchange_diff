using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PublicFolderMoveSyncContext : SyncContext
	{
		public PublicFolderMoveSyncContext(ISourceMailbox sourceMailbox, FolderMap sourceFolderMap, IDestinationMailbox destinationMailbox, FolderMap targetFolderMap) : base(sourceFolderMap, targetFolderMap)
		{
			this.sourceMailbox = sourceMailbox;
			this.destinationMailbox = destinationMailbox;
		}

		public override byte[] GetSourceEntryIdFromTargetFolder(FolderRecWrapper targetFolder)
		{
			return this.sourceMailbox.GetSessionSpecificEntryId(targetFolder.EntryId);
		}

		public override FolderRecWrapper GetTargetFolderBySourceId(byte[] sourceId)
		{
			return base.TargetFolderMap[this.destinationMailbox.GetSessionSpecificEntryId(sourceId)];
		}

		private readonly ISourceMailbox sourceMailbox;

		private readonly IDestinationMailbox destinationMailbox;
	}
}
