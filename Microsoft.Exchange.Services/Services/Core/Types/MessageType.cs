using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(PostReplyItemBaseType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Message")]
	[XmlInclude(typeof(MeetingMessageType))]
	[XmlInclude(typeof(MeetingCancellationMessageType))]
	[XmlInclude(typeof(MeetingResponseMessageType))]
	[XmlInclude(typeof(MeetingRequestMessageType))]
	[XmlInclude(typeof(ResponseObjectCoreType))]
	[XmlInclude(typeof(ResponseObjectType))]
	[XmlInclude(typeof(PostReplyItemType))]
	[XmlInclude(typeof(RemoveItemType))]
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
	[XmlInclude(typeof(DeclineItemType))]
	[XmlInclude(typeof(TentativelyAcceptItemType))]
	[XmlInclude(typeof(AcceptItemType))]
	[XmlInclude(typeof(AddItemToMyCalendarType))]
	[XmlInclude(typeof(ProposeNewTimeType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "Message")]
	[KnownType(typeof(SmartResponseBaseType))]
	[KnownType(typeof(ProposeNewTimeType))]
	[KnownType(typeof(MeetingCancellationMessageType))]
	[KnownType(typeof(MeetingResponseMessageType))]
	[KnownType(typeof(MeetingRequestMessageType))]
	[KnownType(typeof(ResponseObjectCoreType))]
	[KnownType(typeof(ResponseObjectType))]
	[KnownType(typeof(PostReplyItemBaseType))]
	[KnownType(typeof(PostReplyItemType))]
	[KnownType(typeof(RemoveItemType))]
	[KnownType(typeof(ReferenceItemResponseType))]
	[KnownType(typeof(AcceptSharingInvitationType))]
	[KnownType(typeof(SuppressReadReceiptType))]
	[KnownType(typeof(MeetingMessageType))]
	[KnownType(typeof(SmartResponseType))]
	[KnownType(typeof(CancelCalendarItemType))]
	[KnownType(typeof(ForwardItemType))]
	[KnownType(typeof(ReplyAllToItemType))]
	[KnownType(typeof(ReplyToItemType))]
	[KnownType(typeof(WellKnownResponseObjectType))]
	[KnownType(typeof(DeclineItemType))]
	[KnownType(typeof(TentativelyAcceptItemType))]
	[KnownType(typeof(AcceptItemType))]
	[KnownType(typeof(AddItemToMyCalendarType))]
	[Serializable]
	public class MessageType : ItemType, IRelatedItemInfo
	{
		internal new static MessageType CreateFromStoreObjectType(StoreObjectType storeObjectType)
		{
			if (MessageType.createMethods.Member.ContainsKey(storeObjectType))
			{
				return MessageType.createMethods.Member[storeObjectType]();
			}
			return MessageType.createMethods.Member[StoreObjectType.Message]();
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public SingleRecipientType Sender
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SingleRecipientType>(MessageSchema.Sender);
			}
			set
			{
				base.PropertyBag[MessageSchema.Sender] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressWrapper[] ToRecipients
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper[]>(MessageSchema.ToRecipients);
			}
			set
			{
				base.PropertyBag[MessageSchema.ToRecipients] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressWrapper[] CcRecipients
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper[]>(MessageSchema.CcRecipients);
			}
			set
			{
				base.PropertyBag[MessageSchema.CcRecipients] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressWrapper[] BccRecipients
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper[]>(MessageSchema.BccRecipients);
			}
			set
			{
				base.PropertyBag[MessageSchema.BccRecipients] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public bool? IsReadReceiptRequested
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.IsReadReceiptRequested);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(MessageSchema.IsReadReceiptRequested, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsReadReceiptRequestedSpecified
		{
			get
			{
				return base.IsSet(MessageSchema.IsReadReceiptRequested);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public bool? IsDeliveryReceiptRequested
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.IsDeliveryReceiptRequested);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(MessageSchema.IsDeliveryReceiptRequested, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsDeliveryReceiptRequestedSpecified
		{
			get
			{
				return base.IsSet(MessageSchema.IsDeliveryReceiptRequested);
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		[XmlElement(DataType = "base64Binary")]
		public byte[] ConversationIndex
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<byte[]>(MessageSchema.ConversationIndex);
			}
			set
			{
				base.PropertyBag[MessageSchema.ConversationIndex] = value;
			}
		}

		[DataMember(Name = "ConversationIndex", EmitDefaultValue = false, Order = 7)]
		[XmlIgnore]
		public string ConversationIndexString
		{
			get
			{
				byte[] conversationIndex = this.ConversationIndex;
				if (conversationIndex == null)
				{
					return null;
				}
				return Convert.ToBase64String(conversationIndex);
			}
			set
			{
				this.ConversationIndex = ((value != null) ? Convert.FromBase64String(value) : null);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 8)]
		public string ConversationTopic
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MessageSchema.ConversationTopic);
			}
			set
			{
				base.PropertyBag[MessageSchema.ConversationTopic] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 9)]
		public SingleRecipientType From
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SingleRecipientType>(MessageSchema.From);
			}
			set
			{
				base.PropertyBag[MessageSchema.From] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 10)]
		public string InternetMessageId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MessageSchema.InternetMessageId);
			}
			set
			{
				base.PropertyBag[MessageSchema.InternetMessageId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 11)]
		public bool? IsRead
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.IsRead);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(MessageSchema.IsRead, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsReadSpecified
		{
			get
			{
				return base.IsSet(MessageSchema.IsRead);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 12)]
		public bool? IsResponseRequested
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.IsResponseRequested);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(MessageSchema.IsResponseRequested, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsResponseRequestedSpecified
		{
			get
			{
				return base.IsSet(MessageSchema.IsResponseRequested);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 13)]
		public string References
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MessageSchema.References);
			}
			set
			{
				base.PropertyBag[MessageSchema.References] = value;
			}
		}

		[XmlArrayItem("Mailbox", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 14)]
		public EmailAddressWrapper[] ReplyTo
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper[]>(MessageSchema.ReplyTo);
			}
			set
			{
				base.PropertyBag[MessageSchema.ReplyTo] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 15)]
		public SingleRecipientType ReceivedBy
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SingleRecipientType>(MessageSchema.ReceivedBy);
			}
			set
			{
				base.PropertyBag[MessageSchema.ReceivedBy] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 16)]
		public SingleRecipientType ReceivedRepresenting
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SingleRecipientType>(MessageSchema.ReceivedRepresenting);
			}
			set
			{
				base.PropertyBag[MessageSchema.ReceivedRepresenting] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 17)]
		public ApprovalRequestDataType ApprovalRequestData
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ApprovalRequestDataType>(MessageSchema.ApprovalRequestData);
			}
			set
			{
				base.PropertyBag[MessageSchema.ApprovalRequestData] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 18)]
		public VotingInformationType VotingInformation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<VotingInformationType>(MessageSchema.VotingInformation);
			}
			set
			{
				base.PropertyBag[MessageSchema.VotingInformation] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 19)]
		[XmlIgnore]
		public bool? RelyOnConversationIndex
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.RelyOnConversationIndex);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(MessageSchema.RelyOnConversationIndex, value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 20)]
		public ReminderMessageDataType ReminderMessageData
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ReminderMessageDataType>(MessageSchema.ReminderMessageData);
			}
			set
			{
				base.PropertyBag[MessageSchema.ReminderMessageData] = value;
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 21)]
		public ModernReminderType[] ModernReminders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ModernReminderType[]>(MessageSchema.ModernReminders);
			}
			set
			{
				base.PropertyBag[MessageSchema.ModernReminders] = value;
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 22)]
		public int LikeCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int>(MessageSchema.LikeCount);
			}
			set
			{
				base.PropertyBag[MessageSchema.LikeCount] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 23)]
		[XmlIgnore]
		public RecipientCountsType RecipientCounts
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RecipientCountsType>(MessageSchema.RecipientCounts);
			}
			set
			{
				base.PropertyBag[MessageSchema.RecipientCounts] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 24)]
		[XmlIgnore]
		public EmailAddressWrapper[] Likers
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper[]>(MessageSchema.Likers);
			}
			set
			{
				base.PropertyBag[MessageSchema.Likers] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 25)]
		[XmlIgnore]
		public bool? IsGroupEscalationMessage
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.IsGroupEscalationMessage);
			}
			set
			{
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.Message;
			}
		}

		private static LazyMember<Dictionary<StoreObjectType, Func<MessageType>>> createMethods = new LazyMember<Dictionary<StoreObjectType, Func<MessageType>>>(delegate()
		{
			Dictionary<StoreObjectType, Func<MessageType>> dictionary = new Dictionary<StoreObjectType, Func<MessageType>>();
			dictionary.Add(StoreObjectType.MeetingCancellation, () => new MeetingCancellationMessageType());
			dictionary.Add(StoreObjectType.MeetingMessage, () => new MeetingMessageType());
			dictionary.Add(StoreObjectType.MeetingRequest, () => new MeetingRequestMessageType());
			dictionary.Add(StoreObjectType.MeetingResponse, () => new MeetingResponseMessageType());
			dictionary.Add(StoreObjectType.Message, () => new MessageType());
			dictionary.Add(StoreObjectType.Unknown, () => new MessageType());
			return dictionary;
		});
	}
}
