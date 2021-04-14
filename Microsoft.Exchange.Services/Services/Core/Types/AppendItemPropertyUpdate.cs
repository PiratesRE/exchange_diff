using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "AppendToItemField", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "AppendToItemFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class AppendItemPropertyUpdate : AppendPropertyUpdate
	{
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("DistributionList", typeof(DistributionListType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("Task", typeof(TaskType))]
		[DataMember(Name = "Item", IsRequired = true)]
		[XmlElement("CalendarItem", typeof(EwsCalendarItemType))]
		[XmlElement("Contact", typeof(ContactItemType))]
		[XmlElement("Item", typeof(ItemType))]
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
