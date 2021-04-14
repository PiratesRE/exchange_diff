using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "SetItemField", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SetItemFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class SetItemPropertyUpdate : SetPropertyUpdate
	{
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("DistributionList", typeof(DistributionListType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("Item", typeof(ItemType))]
		[XmlElement("Contact", typeof(ContactItemType))]
		[DataMember(Name = "Item", IsRequired = true)]
		[XmlElement("Task", typeof(TaskType))]
		[XmlElement("CalendarItem", typeof(EwsCalendarItemType))]
		public ItemType Item { get; set; }

		internal override ServiceObject ServiceObject
		{
			get
			{
				return this.Item;
			}
		}
	}
}
