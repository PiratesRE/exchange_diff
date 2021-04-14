using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[KnownType(typeof(LinkedCalendarEntry))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(LocalCalendarEntry))]
	public abstract class CalendarEntry
	{
		public CalendarEntry()
		{
		}

		[DataMember]
		public ItemId ItemId { get; set; }

		[DataMember]
		public string CalendarName { get; set; }

		[DataMember]
		public CalendarColor CalendarColor { get; set; }

		[DataMember]
		public string ParentGroupId { get; set; }

		[DataMember]
		public bool IsGroupMailboxCalendar { get; set; }

		[DataMember]
		public CalendarFolderTypeEnum CalendarFolderType { get; set; }
	}
}
