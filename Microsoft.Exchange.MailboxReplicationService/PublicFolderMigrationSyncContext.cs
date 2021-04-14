using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PublicFolderMigrationSyncContext : SyncContext
	{
		public PublicFolderMigrationSyncContext(ISourceMailbox sourceDatabase, FolderMap sourceFolderMap, IDestinationMailbox destinationMailbox, FolderMap targetFolderMap, bool isTargetPrimaryHierarchyMailbox) : base(sourceFolderMap, targetFolderMap)
		{
			this.sourceDatabase = sourceDatabase;
			this.destinationMailbox = destinationMailbox;
			this.isTargetPrimaryHierarchyMailbox = isTargetPrimaryHierarchyMailbox;
		}

		public override byte[] GetSourceEntryIdFromTargetFolder(FolderRecWrapper targetFolder)
		{
			return this.sourceDatabase.GetSessionSpecificEntryId(targetFolder.EntryId);
		}

		public override FolderRecWrapper GetTargetFolderBySourceId(byte[] sourceId)
		{
			FolderMap sourceFolderMap = base.SourceFolderMap;
			FolderMapping folderMapping = base.SourceFolderMap[sourceId] as FolderMapping;
			if (folderMapping != null && folderMapping.IsSystemPublicFolder)
			{
				FolderHierarchy folderHierarchy = base.TargetFolderMap as FolderHierarchy;
				return folderHierarchy.GetWellKnownFolder(folderMapping.WKFType);
			}
			return base.TargetFolderMap[this.destinationMailbox.GetSessionSpecificEntryId(sourceId)];
		}

		public override FolderRecWrapper GetTargetParentFolderBySourceParentId(byte[] sourceParentId)
		{
			if (!this.isTargetPrimaryHierarchyMailbox)
			{
				return ((FolderHierarchy)base.TargetFolderMap).GetWellKnownFolder(WellKnownFolderType.IpmSubtree);
			}
			return this.GetTargetFolderBySourceId(sourceParentId);
		}

		public override FolderRecWrapper CreateSourceFolderRec(FolderRec fRec)
		{
			return new FolderMapping(fRec);
		}

		public override FolderRecWrapper CreateTargetFolderRec(FolderRecWrapper sourceFolderRec)
		{
			return new FolderMapping(((FolderMapping)sourceFolderRec).FolderRec);
		}

		private readonly ISourceMailbox sourceDatabase;

		private readonly IDestinationMailbox destinationMailbox;

		private readonly bool isTargetPrimaryHierarchyMailbox;
	}
}
