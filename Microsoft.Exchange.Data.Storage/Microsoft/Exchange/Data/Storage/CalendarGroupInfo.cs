using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarGroupInfo : FolderTreeDataInfo
	{
		public string GroupName { get; private set; }

		public Guid GroupClassId { get; private set; }

		public CalendarGroupType GroupType { get; private set; }

		public List<CalendarGroupEntryInfo> Calendars { get; private set; }

		public CalendarGroupInfo(string groupName, VersionedId id, Guid groupClassId, CalendarGroupType groupType, byte[] groupOrdinal, ExDateTime lastModifiedTime) : base(id, groupOrdinal, lastModifiedTime)
		{
			Util.ThrowOnNullArgument(groupName, "groupName");
			EnumValidator.ThrowIfInvalid<CalendarGroupType>(groupType, "groupType");
			this.GroupName = groupName;
			this.GroupClassId = groupClassId;
			this.GroupType = groupType;
			this.Calendars = new List<CalendarGroupEntryInfo>();
		}
	}
}
