using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FindItemParentType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class FindItemParentWrapper : FindParentWrapperBase
	{
		public FindItemParentWrapper()
		{
		}

		internal FindItemParentWrapper(ItemType[] items, BasePageResult paging) : base(paging)
		{
			this.Items = items;
			this.Groups = null;
		}

		internal FindItemParentWrapper(GroupType[] groups, BasePageResult paging) : base(paging)
		{
			this.Items = null;
			this.Groups = groups;
		}

		[XmlArrayItem("Item", typeof(ItemType))]
		[XmlArrayItem("Contact", typeof(ContactItemType))]
		[XmlArrayItem("DistributionList", typeof(DistributionListType))]
		[XmlArrayItem("Message", typeof(MessageType))]
		[XmlArrayItem("CalendarItem", typeof(EwsCalendarItemType))]
		[XmlArrayItem("MeetingMessage", typeof(MeetingMessageType))]
		[XmlArrayItem("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlArrayItem("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlArrayItem("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlArrayItem("PostItem", typeof(PostItemType))]
		[XmlArrayItem("Task", typeof(TaskType))]
		[XmlArray("Items")]
		[DataMember(Name = "Items")]
		public ItemType[] Items { get; set; }

		[XmlArray("Groups")]
		[XmlArrayItem("GroupedItems")]
		[DataMember(Name = "Groups")]
		public GroupType[] Groups { get; set; }
	}
}
