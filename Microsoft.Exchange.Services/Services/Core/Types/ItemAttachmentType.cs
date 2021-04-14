using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ItemAttachment")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ItemAttachmentType : AttachmentType
	{
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("Task", typeof(TaskType))]
		[XmlElement("Item", typeof(ItemType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("Contact", typeof(ContactItemType))]
		[DataMember(Name = "Item", IsRequired = true)]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[XmlElement("CalendarItem", typeof(EwsCalendarItemType))]
		public ItemType Item { get; set; }

		[XmlIgnore]
		[DataMember(Name = "EmbeddedItemClass", IsRequired = false)]
		public string EmbeddedItemClass { get; set; }
	}
}
