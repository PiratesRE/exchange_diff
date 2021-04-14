using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types.Conversations
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ConversationThread")]
	[Serializable]
	public class ConversationThreadType : IConversationDataResponse
	{
		[DataMember(Name = "ThreadId", IsRequired = true, Order = 1)]
		public ItemId ThreadId { get; set; }

		[DataMember(Name = "ConversationNodes", EmitDefaultValue = false, Order = 2)]
		public ConversationNode[] ConversationNodes
		{
			get
			{
				if (this.conversationNodes != null && this.conversationNodes.Count > 0)
				{
					return this.conversationNodes.ToArray();
				}
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.conversationNodes = null;
					return;
				}
				this.conversationNodes = new List<ConversationNode>(value);
			}
		}

		[DataMember(Name = "TotalConversationNodesCount", IsRequired = false, Order = 3)]
		public int TotalConversationNodesCount { get; set; }

		[DataMember(Name = "ToRecipients", EmitDefaultValue = false, Order = 4)]
		public EmailAddressWrapper[] ToRecipients
		{
			get
			{
				if (this.toRecipients != null && this.toRecipients.Count > 0)
				{
					return this.toRecipients.ToArray();
				}
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.toRecipients = null;
					return;
				}
				this.toRecipients = new List<EmailAddressWrapper>(value);
			}
		}

		[DataMember(Name = "CcRecipients", EmitDefaultValue = false, Order = 5)]
		public EmailAddressWrapper[] CcRecipients
		{
			get
			{
				if (this.ccRecipients != null && this.ccRecipients.Count > 0)
				{
					return this.ccRecipients.ToArray();
				}
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.ccRecipients = null;
					return;
				}
				this.ccRecipients = new List<EmailAddressWrapper>(value);
			}
		}

		[DataMember(Name = "LastDeliveryTime", EmitDefaultValue = false, Order = 6)]
		[DateTimeString]
		public string LastDeliveryTime { get; set; }

		[DataMember(Name = "UniqueSenders", EmitDefaultValue = false, Order = 7)]
		public EmailAddressWrapper[] UniqueSenders { get; set; }

		[DataMember(Name = "Preview", EmitDefaultValue = false, Order = 8)]
		public string Preview { get; set; }

		[DataMember(Name = "GlobalHasAttachments", EmitDefaultValue = false, Order = 9)]
		public bool GlobalHasAttachments { get; set; }

		[DataMember(Name = "GlobalHasIrm", EmitDefaultValue = false, Order = 10)]
		public bool GlobalHasIrm { get; set; }

		[DataMember(Name = "GlobalImportance", EmitDefaultValue = false, Order = 11)]
		public ImportanceType GlobalImportance { get; set; }

		[IgnoreDataMember]
		[XmlElement("GlobalIconIndex")]
		public IconIndexType GlobalIconIndex { get; set; }

		[IgnoreDataMember]
		[XmlElement("GlobalFlagStatus")]
		public FlagStatusType GlobalFlagStatus { get; set; }

		[DataMember(Name = "GlobalMessageCount", EmitDefaultValue = false, Order = 12)]
		public int GlobalMessageCount { get; set; }

		[DataMember(Name = "UnreadCount", EmitDefaultValue = false, Order = 13)]
		public int UnreadCount { get; set; }

		[DataMember(Name = "InitialMessage", EmitDefaultValue = false, Order = 14)]
		public ConversationNode InitialMessage { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 15)]
		[XmlArrayItem("ItemId", typeof(ItemId), IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), IsNullable = false)]
		public BaseItemId[] GlobalItemIds { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 16)]
		[XmlArrayItem("Int16", IsNullable = false)]
		public short[] GlobalRichContent { get; set; }

		[XmlArrayItem("ItemClass", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 17)]
		public string[] GlobalItemClasses { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 18)]
		[XmlArrayItem("ItemId", typeof(ItemId), IsNullable = false)]
		public BaseItemId[] DraftItemIds { get; set; }

		private List<ConversationNode> conversationNodes;

		private List<EmailAddressWrapper> toRecipients;

		private List<EmailAddressWrapper> ccRecipients;
	}
}
