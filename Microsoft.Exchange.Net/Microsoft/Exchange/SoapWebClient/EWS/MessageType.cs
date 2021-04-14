using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(MeetingMessageType))]
	[XmlInclude(typeof(MeetingCancellationMessageType))]
	[XmlInclude(typeof(MeetingResponseMessageType))]
	[XmlInclude(typeof(MeetingRequestMessageType))]
	[XmlInclude(typeof(ResponseObjectCoreType))]
	[XmlInclude(typeof(ResponseObjectType))]
	[XmlInclude(typeof(PostReplyItemBaseType))]
	[XmlInclude(typeof(PostReplyItemType))]
	[XmlInclude(typeof(AddItemToMyCalendarType))]
	[XmlInclude(typeof(RemoveItemType))]
	[XmlInclude(typeof(ProposeNewTimeType))]
	[XmlInclude(typeof(ReferenceItemResponseType))]
	[XmlInclude(typeof(AcceptSharingInvitationType))]
	[XmlInclude(typeof(SuppressReadReceiptType))]
	[XmlInclude(typeof(SmartResponseBaseType))]
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
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class MessageType : ItemType
	{
		public SingleRecipientType Sender;

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] ToRecipients;

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] CcRecipients;

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] BccRecipients;

		public bool IsReadReceiptRequested;

		[XmlIgnore]
		public bool IsReadReceiptRequestedSpecified;

		public bool IsDeliveryReceiptRequested;

		[XmlIgnore]
		public bool IsDeliveryReceiptRequestedSpecified;

		[XmlElement(DataType = "base64Binary")]
		public byte[] ConversationIndex;

		public string ConversationTopic;

		public SingleRecipientType From;

		public string InternetMessageId;

		public bool IsRead;

		[XmlIgnore]
		public bool IsReadSpecified;

		public bool IsResponseRequested;

		[XmlIgnore]
		public bool IsResponseRequestedSpecified;

		public string References;

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] ReplyTo;

		public SingleRecipientType ReceivedBy;

		public SingleRecipientType ReceivedRepresenting;

		public ApprovalRequestDataType ApprovalRequestData;

		public VotingInformationType VotingInformation;

		public ReminderMessageDataType ReminderMessageData;
	}
}
