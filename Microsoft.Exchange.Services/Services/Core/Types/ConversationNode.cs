using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ConversationNode")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ConversationNode
	{
		internal void AddItem(ItemType item)
		{
			if (this.itemsList == null)
			{
				this.itemsList = new List<ItemType>();
			}
			this.itemsList.Add(item);
		}

		internal int ItemCount
		{
			get
			{
				if (this.itemsList == null)
				{
					return 0;
				}
				return this.itemsList.Count;
			}
		}

		[XmlElement("InternetMessageId", IsNullable = false)]
		[DataMember(Name = "InternetMessageId", IsRequired = true, Order = 1)]
		public string InternetMessageId { get; set; }

		[XmlElement("ParentInternetMessageId", IsNullable = false)]
		[DataMember(Name = "ParentInternetMessageId", EmitDefaultValue = false, IsRequired = false, Order = 2)]
		public string ParentInternetMessageId { get; set; }

		[XmlArrayItem("Message", typeof(MessageType))]
		[XmlArrayItem("PostItem", typeof(PostItemType))]
		[XmlArrayItem("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlArrayItem("MeetingMessage", typeof(MeetingMessageType))]
		[XmlArrayItem("CalendarItem", typeof(EwsCalendarItemType))]
		[XmlArrayItem("Contact", typeof(ContactItemType))]
		[XmlArrayItem("DistributionList", typeof(DistributionListType))]
		[XmlArrayItem("Item", typeof(ItemType))]
		[XmlArrayItem("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlArrayItem("Task", typeof(TaskType))]
		[XmlArray("Items")]
		[XmlArrayItem("MeetingResponse", typeof(MeetingResponseMessageType))]
		[DataMember(Name = "Items", IsRequired = true, Order = 3)]
		public ItemType[] Items
		{
			get
			{
				if (this.itemsList == null)
				{
					return null;
				}
				return this.itemsList.ToArray();
			}
			set
			{
				this.itemsList = ((value != null) ? new List<ItemType>(value) : null);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "NewParticipants", EmitDefaultValue = false, Order = 4)]
		public EmailAddressWrapper[] NewParticipants
		{
			get
			{
				if (this.newParticipants != null && this.newParticipants.Count > 0)
				{
					return this.newParticipants.ToArray();
				}
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.newParticipants = null;
					return;
				}
				this.newParticipants = new List<EmailAddressWrapper>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "InReplyToItem", EmitDefaultValue = false, Order = 5)]
		public InReplyToAdapterType InReplyToItem { get; set; }

		[XmlIgnore]
		[DataMember(Name = "IsRootNode", Order = 6)]
		public bool IsRootNode { get; set; }

		[DataMember(Name = "ForwardMessages", EmitDefaultValue = false, Order = 7)]
		[XmlIgnore]
		public BreadcrumbAdapterType[] ForwardMessages { get; set; }

		[DataMember(Name = "BackwardMessage", EmitDefaultValue = false, Order = 8)]
		[XmlIgnore]
		public BreadcrumbAdapterType BackwardMessage { get; set; }

		private List<ItemType> itemsList;

		private List<EmailAddressWrapper> newParticipants;
	}
}
