using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LinkedCalendarEntry : CalendarEntry
	{
		[DataMember]
		public string OwnerEmailAddress { get; set; }

		[DataMember]
		public string OwnerSipUri { get; set; }

		[DataMember]
		public FolderId SharedFolderId { get; set; }

		[DataMember]
		public bool IsGeneralScheduleCalendar { get; set; }

		[DataMember]
		public bool IsOwnerEmailAddressInvalid { get; set; }
	}
}
