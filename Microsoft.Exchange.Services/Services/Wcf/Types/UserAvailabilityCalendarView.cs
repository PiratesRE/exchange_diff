using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UserAvailabilityCalendarView
	{
		[DataMember]
		public string FreeBusyViewType { get; set; }

		[DataMember]
		public string MergedFreeBusy { get; set; }

		[DataMember]
		public EwsCalendarItemType[] Items { get; set; }

		[DataMember]
		public WorkingHoursType WorkingHours { get; set; }
	}
}
