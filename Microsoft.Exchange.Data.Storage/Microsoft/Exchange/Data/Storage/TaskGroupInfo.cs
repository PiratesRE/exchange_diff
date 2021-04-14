using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskGroupInfo : FolderTreeDataInfo
	{
		public string GroupName { get; private set; }

		public Guid GroupClassId { get; private set; }

		public TaskGroupType GroupType { get; private set; }

		public List<TaskGroupEntryInfo> TaskFolders { get; private set; }

		public TaskGroupInfo(string groupName, VersionedId id, Guid groupClassId, TaskGroupType groupType, byte[] groupOrdinal, ExDateTime lastModifiedTime) : base(id, groupOrdinal, lastModifiedTime)
		{
			Util.ThrowOnNullArgument(groupName, "groupName");
			EnumValidator.ThrowIfInvalid<TaskGroupType>(groupType, "groupType");
			this.GroupName = groupName;
			this.GroupClassId = groupClassId;
			this.GroupType = groupType;
			this.TaskFolders = new List<TaskGroupEntryInfo>();
		}
	}
}
