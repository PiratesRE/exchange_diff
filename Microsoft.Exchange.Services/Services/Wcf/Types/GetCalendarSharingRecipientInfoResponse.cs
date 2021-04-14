using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCalendarSharingRecipientInfoResponse : CalendarActionResponse
	{
		public GetCalendarSharingRecipientInfoResponse(CalendarActionError errorCode) : base(errorCode)
		{
		}

		public GetCalendarSharingRecipientInfoResponse()
		{
		}

		[DataMember]
		public ItemId CalendarId { get; set; }

		[DataMember]
		public CalendarSharingRecipientInfo[] Recipients { get; set; }
	}
}
