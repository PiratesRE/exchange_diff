using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskGroupInfoList : List<TaskGroupInfo>
	{
		public IList<FolderTreeDataInfo> DuplicateNodes { get; private set; }

		public Dictionary<TaskGroupType, TaskGroupInfo> DefaultGroups { get; private set; }

		public Dictionary<StoreObjectId, TaskGroupEntryInfo> TaskFolderMapping { get; private set; }

		public TaskGroupInfoList(IList<FolderTreeDataInfo> duplicateNodes, Dictionary<TaskGroupType, TaskGroupInfo> defaultGroups, Dictionary<StoreObjectId, TaskGroupEntryInfo> taskFolderMapping)
		{
			Util.ThrowOnNullArgument(duplicateNodes, "duplicateNodes");
			Util.ThrowOnNullArgument(defaultGroups, "defaultGroups");
			Util.ThrowOnNullArgument(taskFolderMapping, "taskFolderMapping");
			this.DuplicateNodes = duplicateNodes;
			this.DefaultGroups = defaultGroups;
			this.TaskFolderMapping = taskFolderMapping;
		}
	}
}
