using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GroupType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public int GroupIndex { get; set; }

		[XmlArray("Items")]
		[XmlArrayItem("Message", typeof(MessageType))]
		[XmlArrayItem("CalendarItem", typeof(EwsCalendarItemType))]
		[XmlArrayItem("Contact", typeof(ContactItemType))]
		[XmlArrayItem("DistributionList", typeof(DistributionListType))]
		[XmlArrayItem("Item", typeof(ItemType))]
		[XmlArrayItem("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlArrayItem("MeetingMessage", typeof(MeetingMessageType))]
		[XmlArrayItem("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlArrayItem("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlArrayItem("PostItem", typeof(PostItemType))]
		[XmlArrayItem("Task", typeof(TaskType))]
		[DataMember(Name = "Items", Order = 2)]
		public ItemType[] Items { get; set; }
	}
}
