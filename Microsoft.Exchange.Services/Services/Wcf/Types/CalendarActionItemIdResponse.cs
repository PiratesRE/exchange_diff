using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarActionItemIdResponse : CalendarActionResponse
	{
		public CalendarActionItemIdResponse(CalendarActionError errorCode) : base(errorCode)
		{
		}

		public CalendarActionItemIdResponse(ItemId calendarEntryId)
		{
			this.NewCalendarEntryId = calendarEntryId;
		}

		[DataMember]
		public ItemId NewCalendarEntryId { get; set; }
	}
}
