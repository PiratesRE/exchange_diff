using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SetItemFieldType : ItemChangeDescriptionType
	{
		[XmlElement("Item", typeof(ItemType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("DistributionList", typeof(DistributionListType))]
		[XmlElement("Contact", typeof(ContactItemType))]
		[XmlElement("Task", typeof(TaskType))]
		[XmlElement("CalendarItem", typeof(CalendarItemType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("Message", typeof(MessageType))]
		public ItemType Item1
		{
			get
			{
				return this.item1Field;
			}
			set
			{
				this.item1Field = value;
			}
		}

		private ItemType item1Field;
	}
}
