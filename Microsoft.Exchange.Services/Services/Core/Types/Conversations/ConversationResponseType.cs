using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types.Conversations
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Conversation")]
	[Serializable]
	public class ConversationResponseType : IConversationDataResponse, IConversationResponseType
	{
		[DataMember(Name = "ConversationId", IsRequired = true, Order = 1)]
		[XmlElement("ConversationId", IsNullable = false)]
		public ItemId ConversationId { get; set; }

		[IgnoreDataMember]
		[XmlElement(DataType = "base64Binary")]
		public byte[] SyncState { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SyncState", EmitDefaultValue = false, Order = 2)]
		public string SyncStateString
		{
			get
			{
				byte[] syncState = this.SyncState;
				if (syncState == null)
				{
					return null;
				}
				return Convert.ToBase64String(syncState);
			}
			set
			{
				this.SyncState = (string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value));
			}
		}

		[XmlArrayItem("ConversationNode", typeof(ConversationNode), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "ConversationNodes", EmitDefaultValue = false, Order = 3)]
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

		[XmlIgnore]
		[DataMember(Name = "TotalConversationNodesCount", IsRequired = false, Order = 4)]
		public int TotalConversationNodesCount { get; set; }

		[DataMember(Name = "ToRecipients", EmitDefaultValue = false, Order = 6)]
		[XmlIgnore]
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

		[DataMember(Name = "CcRecipients", EmitDefaultValue = false, Order = 7)]
		[XmlIgnore]
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

		[DataMember(Name = "LastModifiedTime", IsRequired = false, Order = 8)]
		[XmlIgnore]
		[DateTimeString]
		public string LastModifiedTime { get; set; }

		[XmlIgnore]
		[DataMember(Name = "CanDelete", IsRequired = false, Order = 9)]
		public bool CanDelete { get; set; }

		private List<ConversationNode> conversationNodes;

		private List<EmailAddressWrapper> toRecipients;

		private List<EmailAddressWrapper> ccRecipients;
	}
}
