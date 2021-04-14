using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetRemindersResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetRemindersResponse : ResponseMessage
	{
		public GetRemindersResponse()
		{
		}

		internal GetRemindersResponse(ServiceResultCode code, ServiceError error, ReminderType[] reminders) : base(code, error)
		{
			this.Reminders = reminders;
		}

		[XmlArrayItem("Reminder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public ReminderType[] Reminders { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetRemindersResponseMessage;
		}
	}
}
