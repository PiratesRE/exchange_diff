using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskGroupEntryInfo : FolderTreeDataInfo
	{
		public string FolderName { get; private set; }

		public StoreObjectId TaskFolderId { get; private set; }

		public Guid ParentGroupClassId { get; private set; }

		public TaskGroupEntryInfo(string folderName, VersionedId id, StoreObjectId taskFolderId, Guid parentGroupId, byte[] taskFolderOrdinal, ExDateTime lastModifiedTime) : base(id, taskFolderOrdinal, lastModifiedTime)
		{
			Util.ThrowOnNullArgument(folderName, "folderName");
			this.TaskFolderId = taskFolderId;
			this.FolderName = folderName;
			this.ParentGroupClassId = parentGroupId;
		}
	}
}
