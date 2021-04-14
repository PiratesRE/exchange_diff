using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	internal class CalendarItemNotificationPayload : NotificationPayloadBase
	{
		[DataMember]
		public string FolderId { get; set; }

		[DataMember]
		public EwsCalendarItemType Item { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool Reload { get; set; }
	}
}
