using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(SmartResponseBaseType))]
	[XmlInclude(typeof(PostReplyItemType))]
	[XmlInclude(typeof(RemoveItemType))]
	[XmlInclude(typeof(ProposeNewTimeType))]
	[XmlInclude(typeof(ReferenceItemResponseType))]
	[XmlInclude(typeof(AcceptSharingInvitationType))]
	[XmlInclude(typeof(SuppressReadReceiptType))]
	[XmlInclude(typeof(MeetingMessageType))]
	[XmlInclude(typeof(SmartResponseType))]
	[XmlInclude(typeof(CancelCalendarItemType))]
	[XmlInclude(typeof(ForwardItemType))]
	[XmlInclude(typeof(ReplyAllToItemType))]
	[XmlInclude(typeof(ReplyToItemType))]
	[XmlInclude(typeof(WellKnownResponseObjectType))]
	[XmlInclude(typeof(MeetingRegistrationResponseObjectType))]
	[XmlInclude(typeof(DeclineItemType))]
	[XmlInclude(typeof(TentativelyAcceptItemType))]
	[XmlInclude(typeof(AcceptItemType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(PostReplyItemBaseType))]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(AddItemToMyCalendarType))]
	[XmlInclude(typeof(MeetingCancellationMessageType))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(MeetingResponseMessageType))]
	[XmlInclude(typeof(MeetingRequestMessageType))]
	[XmlInclude(typeof(ResponseObjectCoreType))]
	[XmlInclude(typeof(ResponseObjectType))]
	[Serializable]
	public class MessageType : ItemType
	{
		public SingleRecipientType Sender
		{
			get
			{
				return this.senderField;
			}
			set
			{
				this.senderField = value;
			}
		}

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] ToRecipients
		{
			get
			{
				return this.toRecipientsField;
			}
			set
			{
				this.toRecipientsField = value;
			}
		}

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] CcRecipients
		{
			get
			{
				return this.ccRecipientsField;
			}
			set
			{
				this.ccRecipientsField = value;
			}
		}

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] BccRecipients
		{
			get
			{
				return this.bccRecipientsField;
			}
			set
			{
				this.bccRecipientsField = value;
			}
		}

		public bool IsReadReceiptRequested
		{
			get
			{
				return this.isReadReceiptRequestedField;
			}
			set
			{
				this.isReadReceiptRequestedField = value;
			}
		}

		[XmlIgnore]
		public bool IsReadReceiptRequestedSpecified
		{
			get
			{
				return this.isReadReceiptRequestedFieldSpecified;
			}
			set
			{
				this.isReadReceiptRequestedFieldSpecified = value;
			}
		}

		public bool IsDeliveryReceiptRequested
		{
			get
			{
				return this.isDeliveryReceiptRequestedField;
			}
			set
			{
				this.isDeliveryReceiptRequestedField = value;
			}
		}

		[XmlIgnore]
		public bool IsDeliveryReceiptRequestedSpecified
		{
			get
			{
				return this.isDeliveryReceiptRequestedFieldSpecified;
			}
			set
			{
				this.isDeliveryReceiptRequestedFieldSpecified = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] ConversationIndex
		{
			get
			{
				return this.conversationIndexField;
			}
			set
			{
				this.conversationIndexField = value;
			}
		}

		public string ConversationTopic
		{
			get
			{
				return this.conversationTopicField;
			}
			set
			{
				this.conversationTopicField = value;
			}
		}

		public SingleRecipientType From
		{
			get
			{
				return this.fromField;
			}
			set
			{
				this.fromField = value;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return this.internetMessageIdField;
			}
			set
			{
				this.internetMessageIdField = value;
			}
		}

		public bool IsRead
		{
			get
			{
				return this.isReadField;
			}
			set
			{
				this.isReadField = value;
			}
		}

		[XmlIgnore]
		public bool IsReadSpecified
		{
			get
			{
				return this.isReadFieldSpecified;
			}
			set
			{
				this.isReadFieldSpecified = value;
			}
		}

		public bool IsResponseRequested
		{
			get
			{
				return this.isResponseRequestedField;
			}
			set
			{
				this.isResponseRequestedField = value;
			}
		}

		[XmlIgnore]
		public bool IsResponseRequestedSpecified
		{
			get
			{
				return this.isResponseRequestedFieldSpecified;
			}
			set
			{
				this.isResponseRequestedFieldSpecified = value;
			}
		}

		public string References
		{
			get
			{
				return this.referencesField;
			}
			set
			{
				this.referencesField = value;
			}
		}

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] ReplyTo
		{
			get
			{
				return this.replyToField;
			}
			set
			{
				this.replyToField = value;
			}
		}

		public SingleRecipientType ReceivedBy
		{
			get
			{
				return this.receivedByField;
			}
			set
			{
				this.receivedByField = value;
			}
		}

		public SingleRecipientType ReceivedRepresenting
		{
			get
			{
				return this.receivedRepresentingField;
			}
			set
			{
				this.receivedRepresentingField = value;
			}
		}

		public ApprovalRequestDataType ApprovalRequestData
		{
			get
			{
				return this.approvalRequestDataField;
			}
			set
			{
				this.approvalRequestDataField = value;
			}
		}

		public VotingInformationType VotingInformation
		{
			get
			{
				return this.votingInformationField;
			}
			set
			{
				this.votingInformationField = value;
			}
		}

		public ReminderMessageDataType ReminderMessageData
		{
			get
			{
				return this.reminderMessageDataField;
			}
			set
			{
				this.reminderMessageDataField = value;
			}
		}

		private SingleRecipientType senderField;

		private EmailAddressType[] toRecipientsField;

		private EmailAddressType[] ccRecipientsField;

		private EmailAddressType[] bccRecipientsField;

		private bool isReadReceiptRequestedField;

		private bool isReadReceiptRequestedFieldSpecified;

		private bool isDeliveryReceiptRequestedField;

		private bool isDeliveryReceiptRequestedFieldSpecified;

		private byte[] conversationIndexField;

		private string conversationTopicField;

		private SingleRecipientType fromField;

		private string internetMessageIdField;

		private bool isReadField;

		private bool isReadFieldSpecified;

		private bool isResponseRequestedField;

		private bool isResponseRequestedFieldSpecified;

		private string referencesField;

		private EmailAddressType[] replyToField;

		private SingleRecipientType receivedByField;

		private SingleRecipientType receivedRepresentingField;

		private ApprovalRequestDataType approvalRequestDataField;

		private VotingInformationType votingInformationField;

		private ReminderMessageDataType reminderMessageDataField;
	}
}
