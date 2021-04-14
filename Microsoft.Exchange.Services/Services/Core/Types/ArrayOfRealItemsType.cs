using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ArrayOfRealItemsType
	{
		[XmlElement("CalendarItem", typeof(EwsCalendarItemType))]
		[XmlElement("DistributionList", typeof(DistributionListType))]
		[XmlElement("Item", typeof(ItemType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("Contact", typeof(ContactItemType))]
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("Task", typeof(TaskType))]
		public ItemType[] Items { get; set; }
	}
}
