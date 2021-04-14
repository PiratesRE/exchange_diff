using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UserAvailabilityInternalResponse
	{
		[DataMember]
		public ResponseMessage ResponseMessage { get; set; }

		[DataMember]
		public UserAvailabilityCalendarView CalendarView { get; set; }
	}
}
