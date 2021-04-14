using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCalendarSharingPermissionsResponse : CalendarActionResponse
	{
		[DataMember]
		public CalendarSharingPermissionInfo[] Recipients { get; set; }

		[DataMember]
		public string AnonymousSharingMaxDetailLevel { get; set; }

		[DataMember]
		public bool IsAnonymousSharingEnabled { get; set; }

		[DataMember]
		public DeliverMeetingRequestsType CurrentDeliveryOption { get; set; }
	}
}
