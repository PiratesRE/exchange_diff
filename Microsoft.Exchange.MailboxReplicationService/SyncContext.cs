using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SyncContext
	{
		public SyncContext(FolderMap sourceFolderMap, FolderMap targetFolderMap)
		{
			this.sourceFolderMap = sourceFolderMap;
			this.targetFolderMap = targetFolderMap;
			this.CopyMessagesCount = default(CopyMessagesCount);
		}

		public int NumberOfHierarchyUpdates { get; set; }

		public CopyMessagesCount CopyMessagesCount { get; set; }

		public FolderMap SourceFolderMap
		{
			get
			{
				return this.sourceFolderMap;
			}
		}

		public FolderMap TargetFolderMap
		{
			get
			{
				return this.targetFolderMap;
			}
		}

		public virtual byte[] GetSourceEntryIdFromTargetFolder(FolderRecWrapper targetFolder)
		{
			return targetFolder.EntryId;
		}

		public virtual FolderRecWrapper GetTargetFolderBySourceId(byte[] sourceId)
		{
			return this.TargetFolderMap[sourceId];
		}

		public virtual FolderRecWrapper GetTargetParentFolderBySourceParentId(byte[] sourceParentId)
		{
			return this.GetTargetFolderBySourceId(sourceParentId);
		}

		public virtual FolderRecWrapper CreateSourceFolderRec(FolderRec fRec)
		{
			return new FolderRecWrapper(fRec);
		}

		public virtual FolderRecWrapper CreateTargetFolderRec(FolderRecWrapper sourceFolderRec)
		{
			return new FolderRecWrapper(sourceFolderRec.FolderRec);
		}

		private readonly FolderMap sourceFolderMap;

		private readonly FolderMap targetFolderMap;
	}
}
