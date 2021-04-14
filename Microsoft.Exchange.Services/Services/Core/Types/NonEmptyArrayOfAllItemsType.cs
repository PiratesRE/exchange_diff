using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(ForwardItemType))]
	[KnownType(typeof(MessageType))]
	[KnownType(typeof(MeetingRequestMessageType))]
	[KnownType(typeof(MeetingResponseMessageType))]
	[KnownType(typeof(MeetingMessageType))]
	[KnownType(typeof(ItemType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(ProposeNewTimeType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(AcceptItemType))]
	[KnownType(typeof(AcceptSharingInvitationType))]
	[KnownType(typeof(EwsCalendarItemType))]
	[KnownType(typeof(CancelCalendarItemType))]
	[KnownType(typeof(ContactItemType))]
	[KnownType(typeof(DeclineItemType))]
	[KnownType(typeof(DistributionListType))]
	[KnownType(typeof(MeetingCancellationMessageType))]
	[KnownType(typeof(PostItemType))]
	[KnownType(typeof(PostReplyItemType))]
	[KnownType(typeof(RemoveItemType))]
	[KnownType(typeof(ReplyAllToItemType))]
	[KnownType(typeof(ReplyToItemType))]
	[KnownType(typeof(SuppressReadReceiptType))]
	[KnownType(typeof(TaskType))]
	[KnownType(typeof(TentativelyAcceptItemType))]
	[KnownType(typeof(AddItemToMyCalendarType))]
	[Serializable]
	public class NonEmptyArrayOfAllItemsType
	{
		[XmlElement("ReplyToItem", typeof(ReplyToItemType))]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("DeclineItem", typeof(DeclineItemType))]
		[XmlElement("ReplyAllToItem", typeof(ReplyAllToItemType))]
		[XmlElement("CalendarItem", typeof(EwsCalendarItemType))]
		[XmlElement("CancelCalendarItem", typeof(CancelCalendarItemType))]
		[XmlElement("SuppressReadReceipt", typeof(SuppressReadReceiptType))]
		[XmlElement("Task", typeof(TaskType))]
		[XmlElement("TentativelyAcceptItem", typeof(TentativelyAcceptItemType))]
		[XmlElement("AddItemToMyCalendar", typeof(AddItemToMyCalendarType))]
		[XmlElement("ProposeNewTime", typeof(ProposeNewTimeType))]
		[XmlElement("Contact", typeof(ContactItemType))]
		[XmlElement("DistributionList", typeof(DistributionListType))]
		[XmlElement("ForwardItem", typeof(ForwardItemType))]
		[XmlElement("Item", typeof(ItemType))]
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("PostReplyItem", typeof(PostReplyItemType))]
		[XmlElement("RemoveItem", typeof(RemoveItemType))]
		[XmlElement("AcceptItem", typeof(AcceptItemType))]
		[XmlElement("AcceptSharingInvitation", typeof(AcceptSharingInvitationType))]
		public ItemType[] Items
		{
			get
			{
				return this.items.ToArray();
			}
			set
			{
				this.items.Clear();
				if (value != null)
				{
					this.items.AddRange(value);
				}
			}
		}

		public void Add(ItemType item)
		{
			this.items.Add(item);
		}

		private List<ItemType> items = new List<ItemType>();
	}
}
