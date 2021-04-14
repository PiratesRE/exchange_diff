using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarGroupInfoList : List<CalendarGroupInfo>
	{
		public IList<FolderTreeDataInfo> DuplicateNodes { get; private set; }

		public Dictionary<CalendarGroupType, CalendarGroupInfo> DefaultGroups { get; private set; }

		public Dictionary<StoreObjectId, LocalCalendarGroupEntryInfo> CalendarFolderMapping { get; private set; }

		public CalendarGroupInfoList(IList<FolderTreeDataInfo> duplicateNodes, Dictionary<CalendarGroupType, CalendarGroupInfo> defaultGroups, Dictionary<StoreObjectId, LocalCalendarGroupEntryInfo> calendarFolderMapping)
		{
			Util.ThrowOnNullArgument(duplicateNodes, "duplicateNodes");
			Util.ThrowOnNullArgument(defaultGroups, "defaultGroups");
			Util.ThrowOnNullArgument(calendarFolderMapping, "calendarFolderMapping");
			this.DuplicateNodes = duplicateNodes;
			this.DefaultGroups = defaultGroups;
			this.CalendarFolderMapping = calendarFolderMapping;
		}
	}
}
