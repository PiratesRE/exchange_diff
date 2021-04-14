using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class NonEmptyArrayOfAllItemsType
	{
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("PostReplyItem", typeof(PostReplyItemType))]
		[XmlElement("RemoveItem", typeof(RemoveItemType))]
		[XmlElement("ReplyAllToItem", typeof(ReplyAllToItemType))]
		[XmlElement("ReplyToItem", typeof(ReplyToItemType))]
		[XmlElement("SuppressReadReceipt", typeof(SuppressReadReceiptType))]
		[XmlElement("Task", typeof(TaskType))]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[XmlElement("Item", typeof(ItemType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("TentativelyAcceptItem", typeof(TentativelyAcceptItemType))]
		[XmlElement("AcceptItem", typeof(AcceptItemType))]
		[XmlElement("AcceptSharingInvitation", typeof(AcceptSharingInvitationType))]
		[XmlElement("CalendarItem", typeof(CalendarItemType))]
		[XmlElement("ForwardItem", typeof(ForwardItemType))]
		[XmlElement("Contact", typeof(ContactItemType))]
		[XmlElement("DeclineItem", typeof(DeclineItemType))]
		[XmlElement("DistributionList", typeof(DistributionListType))]
		[XmlElement("CancelCalendarItem", typeof(CancelCalendarItemType))]
		public ItemType[] Items;
	}
}
