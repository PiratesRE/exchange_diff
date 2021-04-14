using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PublicFolderMap : FolderMap
	{
		public PublicFolderMap(List<FolderRecWrapper> folders) : base(folders)
		{
		}

		public override FolderRecWrapper RootRec
		{
			get
			{
				return null;
			}
		}

		protected override void InsertFolderInternal(FolderRecWrapper rec)
		{
			if (this.folders.ContainsKey(rec.EntryId))
			{
				FolderRecWrapper folderRecWrapper = this.folders[rec.EntryId];
				MrsTracer.Service.Error("Folder {0} is listed more than once in the input folder list", new object[]
				{
					rec.FolderRec.ToString()
				});
				throw new FolderHierarchyContainsDuplicatesPermanentException(rec.FolderRec.ToString(), folderRecWrapper.FolderRec.ToString());
			}
			this.folders[rec.EntryId] = rec;
		}

		protected override void ValidateMap()
		{
		}

		public override void EnumerateSubtree(EnumHierarchyFlags flags, FolderRecWrapper root, FolderMap.EnumFolderCallback callback)
		{
			FolderMap.EnumFolderContext enumFolderContext = new FolderMap.EnumFolderContext();
			enumFolderContext.Result = EnumHierarchyResult.Continue;
			foreach (KeyValuePair<byte[], FolderRecWrapper> keyValuePair in this.folders)
			{
				callback(keyValuePair.Value, enumFolderContext);
			}
		}

		protected override IEnumerable<FolderRecWrapper> GetFolderList(EnumHierarchyFlags flags, FolderRecWrapper folderRec)
		{
			foreach (KeyValuePair<byte[], FolderRecWrapper> kvp in this.folders)
			{
				KeyValuePair<byte[], FolderRecWrapper> keyValuePair = kvp;
				yield return keyValuePair.Value;
			}
			yield break;
		}
	}
}
