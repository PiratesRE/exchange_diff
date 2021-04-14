using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCalendarFoldersResponse : CalendarActionResponse
	{
		internal GetCalendarFoldersResponse(CalendarGroup[] calendarGroups, CalendarFolderType[] calendarFolders)
		{
			this.CalendarGroups = calendarGroups;
			this.CalendarFolders = calendarFolders;
		}

		[DataMember]
		public CalendarGroup[] CalendarGroups { get; set; }

		[DataMember]
		public CalendarFolderType[] CalendarFolders { get; set; }
	}
}
