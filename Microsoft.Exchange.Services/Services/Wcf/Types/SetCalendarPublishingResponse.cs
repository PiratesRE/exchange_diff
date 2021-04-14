using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetCalendarPublishingResponse : CalendarActionResponse
	{
		public SetCalendarPublishingResponse()
		{
		}

		public SetCalendarPublishingResponse(CalendarActionError errorCode) : base(errorCode)
		{
		}

		[DataMember]
		public string BrowseUrl { get; set; }

		[DataMember]
		public string ICalUrl { get; set; }

		[DataMember]
		public string CurrentDetailLevel { get; set; }

		[DataMember]
		public string MaxDetailLevel { get; set; }
	}
}
