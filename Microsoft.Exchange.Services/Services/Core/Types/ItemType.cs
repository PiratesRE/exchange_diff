using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(PostReplyItemBaseType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Item")]
	[KnownType(typeof(PostItemType))]
	[KnownType(typeof(SuppressReadReceiptType))]
	[KnownType(typeof(SmartResponseBaseType))]
	[KnownType(typeof(RemoveItemType))]
	[KnownType(typeof(ReferenceItemResponseType))]
	[KnownType(typeof(PostReplyItemType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "Item")]
	[XmlInclude(typeof(AddItemToMyCalendarType))]
	[KnownType(typeof(ProposeNewTimeType))]
	[XmlInclude(typeof(DistributionListType))]
	[XmlInclude(typeof(ContactItemType))]
	[XmlInclude(typeof(EwsCalendarItemType))]
	[XmlInclude(typeof(MessageType))]
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
	[XmlInclude(typeof(TaskType))]
	[XmlInclude(typeof(ProposeNewTimeType))]
	[XmlInclude(typeof(PostItemType))]
	[KnownType(typeof(TaskType))]
	[KnownType(typeof(DistributionListType))]
	[KnownType(typeof(ContactItemType))]
	[KnownType(typeof(EwsCalendarItemType))]
	[KnownType(typeof(MessageType))]
	[KnownType(typeof(MeetingMessageType))]
	[KnownType(typeof(MeetingCancellationMessageType))]
	[KnownType(typeof(MeetingResponseMessageType))]
	[KnownType(typeof(MeetingRequestMessageType))]
	[KnownType(typeof(ResponseObjectCoreType))]
	[KnownType(typeof(ResponseObjectType))]
	[KnownType(typeof(PostReplyItemBaseType))]
	[KnownType(typeof(AcceptSharingInvitationType))]
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
	public class ItemType : ServiceObject
	{
		internal static ItemType CreateFromStoreObjectType(StoreObjectType storeObjectType)
		{
			if (ItemType.createMethods.Member.ContainsKey(storeObjectType))
			{
				return ItemType.createMethods.Member[storeObjectType]();
			}
			return ItemType.createMethods.Member[StoreObjectType.Message]();
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public MimeContentType MimeContent
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<MimeContentType>(ItemSchema.MimeContent);
			}
			set
			{
				base.PropertyBag[ItemSchema.MimeContent] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public ItemId ItemId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ItemId>(ItemSchema.ItemId);
			}
			set
			{
				base.PropertyBag[ItemSchema.ItemId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public FolderId ParentFolderId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<FolderId>(ItemSchema.ParentFolderId);
			}
			set
			{
				base.PropertyBag[ItemSchema.ParentFolderId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string ItemClass
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.ItemClass);
			}
			set
			{
				base.PropertyBag[ItemSchema.ItemClass] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public string Subject
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.Subject);
			}
			set
			{
				base.PropertyBag[ItemSchema.Subject] = value;
			}
		}

		[IgnoreDataMember]
		[XmlElement]
		public SensitivityType Sensitivity
		{
			get
			{
				if (!this.SensitivitySpecified)
				{
					return SensitivityType.Normal;
				}
				return EnumUtilities.Parse<SensitivityType>(this.SensitivityString);
			}
			set
			{
				this.SensitivityString = EnumUtilities.ToString<SensitivityType>(value);
			}
		}

		[DataMember(Name = "Sensitivity", EmitDefaultValue = false, Order = 6)]
		[XmlIgnore]
		public string SensitivityString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.Sensitivity);
			}
			set
			{
				base.PropertyBag[ItemSchema.Sensitivity] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool SensitivitySpecified
		{
			get
			{
				return base.IsSet(ItemSchema.Sensitivity);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 7)]
		public BodyContentType Body
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BodyContentType>(ItemSchema.Body);
			}
			set
			{
				base.PropertyBag[ItemSchema.Body] = value;
			}
		}

		[XmlArrayItem("ItemAttachment", typeof(ItemAttachmentType), IsNullable = false)]
		[XmlArrayItem("ReferenceAttachment", typeof(ReferenceAttachmentType), IsNullable = false)]
		[XmlArrayItem("FileAttachment", typeof(FileAttachmentType), IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 8)]
		public AttachmentType[] Attachments
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<AttachmentType[]>(ItemSchema.Attachments);
			}
			set
			{
				base.PropertyBag[ItemSchema.Attachments] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 9)]
		public string DateTimeReceived
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.DateTimeReceived);
			}
			set
			{
				base.PropertyBag[ItemSchema.DateTimeReceived] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool DateTimeReceivedSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.DateTimeReceived);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 10)]
		public int? Size
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ItemSchema.Size);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ItemSchema.Size, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool SizeSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.Size);
			}
			set
			{
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 11)]
		public string[] Categories
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ItemSchema.Categories);
			}
			set
			{
				base.PropertyBag[ItemSchema.Categories] = value;
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public ImportanceType Importance
		{
			get
			{
				if (!this.ImportanceSpecified)
				{
					return ImportanceType.Normal;
				}
				return EnumUtilities.Parse<ImportanceType>(this.ImportanceString);
			}
			set
			{
				this.ImportanceString = EnumUtilities.ToString<ImportanceType>(value);
			}
		}

		[DataMember(Name = "Importance", EmitDefaultValue = false, Order = 6)]
		[XmlIgnore]
		public string ImportanceString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.Importance);
			}
			set
			{
				base.PropertyBag[ItemSchema.Importance] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ImportanceSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.Importance);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 13)]
		public string InReplyTo
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.InReplyTo);
			}
			set
			{
				base.PropertyBag[ItemSchema.InReplyTo] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 14)]
		public bool? IsSubmitted
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.IsSubmitted);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.IsSubmitted, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsSubmittedSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IsSubmitted);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 15)]
		public bool? IsDraft
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.IsDraft);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.IsDraft, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsDraftSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IsDraft);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 16)]
		public bool? IsFromMe
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.IsFromMe);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.IsFromMe, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsFromMeSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IsFromMe);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 17)]
		public bool? IsResend
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.IsResend);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.IsResend, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsResendSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IsResend);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 18)]
		public bool? IsUnmodified
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.IsUnmodified);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.IsUnmodified, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsUnmodifiedSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IsUnmodified);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 19)]
		[XmlArrayItem("InternetMessageHeader", IsNullable = false)]
		public InternetHeaderType[] InternetMessageHeaders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<InternetHeaderType[]>(ItemSchema.InternetMessageHeaders);
			}
			set
			{
				base.PropertyBag[ItemSchema.InternetMessageHeaders] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 20)]
		[DateTimeString]
		public string DateTimeSent
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.DateTimeSent);
			}
			set
			{
				base.PropertyBag[ItemSchema.DateTimeSent] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool DateTimeSentSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.DateTimeSent);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 21)]
		[DateTimeString]
		public string DateTimeCreated
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.DateTimeCreated);
			}
			set
			{
				base.PropertyBag[ItemSchema.DateTimeCreated] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool DateTimeCreatedSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.DateTimeCreated);
			}
			set
			{
			}
		}

		[XmlArrayItem("RemoveItem", typeof(RemoveItemType), IsNullable = false)]
		[XmlArrayItem("ReplyToItem", typeof(ReplyToItemType), IsNullable = false)]
		[XmlArrayItem("SuppressReadReceipt", typeof(SuppressReadReceiptType), IsNullable = false)]
		[XmlArrayItem("CancelCalendarItem", typeof(CancelCalendarItemType), IsNullable = false)]
		[XmlArrayItem("ReplyAllToItem", typeof(ReplyAllToItemType), IsNullable = false)]
		[XmlArrayItem("TentativelyAcceptItem", typeof(TentativelyAcceptItemType), IsNullable = false)]
		[XmlArrayItem("AddItemToMyCalendar", typeof(AddItemToMyCalendarType), IsNullable = false)]
		[XmlArrayItem("ProposeNewTime", typeof(ProposeNewTimeType), IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 22)]
		[XmlArrayItem("DeclineItem", typeof(DeclineItemType), IsNullable = false)]
		[XmlArrayItem("AcceptItem", typeof(AcceptItemType), IsNullable = false)]
		[XmlArrayItem("ForwardItem", typeof(ForwardItemType), IsNullable = false)]
		[XmlArrayItem("PostReplyItem", typeof(PostReplyItemType), IsNullable = false)]
		[XmlArrayItem("AcceptSharingInvitation", typeof(AcceptSharingInvitationType), IsNullable = false)]
		public ResponseObjectType[] ResponseObjects
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ResponseObjectType[]>(ItemSchema.ResponseObjects);
			}
			set
			{
				base.PropertyBag[ItemSchema.ResponseObjects] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 23)]
		public string ReminderDueBy
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.ReminderDueBy);
			}
			set
			{
				base.PropertyBag[ItemSchema.ReminderDueBy] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ReminderDueBySpecified
		{
			get
			{
				return base.IsSet(ItemSchema.ReminderDueBy);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 24)]
		public bool? ReminderIsSet
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.ReminderIsSet);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.ReminderIsSet, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ReminderIsSetSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.ReminderIsSet);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public Guid MailboxGuid
		{
			get
			{
				return base.GetValueOrDefault<Guid>(ItemSchema.MailboxGuid);
			}
			set
			{
				this[ItemSchema.MailboxGuid] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 25)]
		public string ReminderNextTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.ReminderNextTime);
			}
			set
			{
				base.PropertyBag[ItemSchema.ReminderNextTime] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ReminderNextTimeSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.ReminderNextTime);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 26)]
		public int? ReminderMinutesBeforeStart
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ItemSchema.ReminderMinutesBeforeStart);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ItemSchema.ReminderMinutesBeforeStart, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool ReminderMinutesBeforeStartSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.ReminderMinutesBeforeStart);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 27)]
		public string DisplayCc
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.DisplayCc);
			}
			set
			{
				base.PropertyBag[ItemSchema.DisplayCc] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 28)]
		public string DisplayTo
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.DisplayTo);
			}
			set
			{
				base.PropertyBag[ItemSchema.DisplayTo] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 29)]
		public bool? HasAttachments
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.HasAttachments);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.HasAttachments, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool HasAttachmentsSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.HasAttachments);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 30)]
		[XmlElement("ExtendedProperty")]
		public ExtendedPropertyType[] ExtendedProperty
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ExtendedPropertyType[]>(ItemSchema.ExtendedProperty);
			}
			set
			{
				base.PropertyBag[ItemSchema.ExtendedProperty] = value;
			}
		}

		[XmlElement(DataType = "language")]
		[DataMember(EmitDefaultValue = false, Order = 31)]
		public string Culture
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.Culture);
			}
			set
			{
				base.PropertyBag[ItemSchema.Culture] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 32)]
		public EffectiveRightsType EffectiveRights
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EffectiveRightsType>(ItemSchema.EffectiveRights);
			}
			set
			{
				base.PropertyBag[ItemSchema.EffectiveRights] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 33)]
		public string LastModifiedName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.LastModifiedName);
			}
			set
			{
				base.PropertyBag[ItemSchema.LastModifiedName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 34)]
		[DateTimeString]
		public string LastModifiedTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.LastModifiedTime);
			}
			set
			{
				base.PropertyBag[ItemSchema.LastModifiedTime] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool LastModifiedTimeSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.LastModifiedTime);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 35)]
		public bool? IsAssociated
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.IsAssociated);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.IsAssociated, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsAssociatedSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IsAssociated);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 36)]
		public string WebClientReadFormQueryString
		{
			get
			{
				StringBuilder valueOrDefault = base.PropertyBag.GetValueOrDefault<StringBuilder>(ItemSchema.WebClientReadFormQueryString);
				if (valueOrDefault != null)
				{
					return valueOrDefault.ToString();
				}
				return null;
			}
			set
			{
				base.PropertyBag[ItemSchema.WebClientReadFormQueryString] = new StringBuilder(value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 37)]
		public string WebClientEditFormQueryString
		{
			get
			{
				StringBuilder valueOrDefault = base.PropertyBag.GetValueOrDefault<StringBuilder>(ItemSchema.WebClientEditFormQueryString);
				if (valueOrDefault != null)
				{
					return valueOrDefault.ToString();
				}
				return null;
			}
			set
			{
				base.PropertyBag[ItemSchema.WebClientEditFormQueryString] = new StringBuilder(value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 38)]
		public ItemId ConversationId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ItemId>(ItemSchema.ConversationId);
			}
			set
			{
				base.PropertyBag[ItemSchema.ConversationId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 39)]
		public BodyContentType UniqueBody
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BodyContentType>(ItemSchema.UniqueBody);
			}
			set
			{
				base.PropertyBag[ItemSchema.UniqueBody] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 40)]
		public FlagType Flag
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<FlagType>(ItemSchema.Flag);
			}
			set
			{
				base.PropertyBag[ItemSchema.Flag] = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		[IgnoreDataMember]
		public byte[] StoreEntryId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<byte[]>(ItemSchema.StoreEntryId);
			}
			set
			{
			}
		}

		[DataMember(Name = "StoreEntryId", EmitDefaultValue = false, Order = 41)]
		[XmlIgnore]
		public string StoreEntryIdString
		{
			get
			{
				byte[] storeEntryId = this.StoreEntryId;
				if (storeEntryId == null)
				{
					return null;
				}
				return Convert.ToBase64String(storeEntryId);
			}
			set
			{
			}
		}

		[XmlElement(DataType = "base64Binary")]
		[IgnoreDataMember]
		public byte[] InstanceKey
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<byte[]>(ItemSchema.InstanceKey);
			}
			set
			{
				base.PropertyBag[ItemSchema.InstanceKey] = value;
			}
		}

		[DataMember(Name = "InstanceKey", EmitDefaultValue = false, Order = 42)]
		[XmlIgnore]
		public string InstanceKeyString
		{
			get
			{
				byte[] instanceKey = this.InstanceKey;
				if (instanceKey == null)
				{
					return null;
				}
				return Convert.ToBase64String(instanceKey);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 43)]
		public BodyContentType NormalizedBody
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BodyContentType>(ItemSchema.NormalizedBody);
			}
			set
			{
				base.PropertyBag[ItemSchema.NormalizedBody] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 44)]
		public EntityExtractionResultType EntityExtractionResult
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EntityExtractionResultType>(ItemSchema.EntityExtractionResult);
			}
			set
			{
			}
		}

		[DataMember(Name = "PolicyTag", EmitDefaultValue = false, Order = 45)]
		public RetentionTagType PolicyTag
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RetentionTagType>(ItemSchema.PolicyTag);
			}
			set
			{
				base.PropertyBag[ItemSchema.PolicyTag] = value;
			}
		}

		[DataMember(Name = "ArchiveTag", EmitDefaultValue = false, Order = 46)]
		public RetentionTagType ArchiveTag
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RetentionTagType>(ItemSchema.ArchiveTag);
			}
			set
			{
				base.PropertyBag[ItemSchema.ArchiveTag] = value;
			}
		}

		[DateTimeString]
		[DataMember(Name = "RetentionDate", EmitDefaultValue = false, Order = 47)]
		public string RetentionDate
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.RetentionDate);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 48)]
		public string Preview
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.Preview);
			}
			set
			{
				base.PropertyBag[ItemSchema.Preview] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 49)]
		public RightsManagementLicenseDataType RightsManagementLicenseData
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RightsManagementLicenseDataType>(ItemSchema.RightsManagementLicenseData);
			}
			set
			{
				base.PropertyBag[ItemSchema.RightsManagementLicenseData] = value;
			}
		}

		[XmlArrayItem("PredictedActionReason", typeof(PredictedActionReasonType), IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Name = "PredictedActionReasons", Order = 52)]
		public PredictedActionReasonType[] PredictedActionReasons
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PredictedActionReasonType[]>(ItemSchema.PredictedActionReasons);
			}
			set
			{
				base.PropertyBag[ItemSchema.PredictedActionReasons] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool PredictedActionReasonsSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.PredictedActionReasons);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 53)]
		public bool? IsClutter
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.IsClutter);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.IsClutter, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsClutterSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IsClutter);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 54)]
		public bool? BlockStatus
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.BlockStatus);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ItemSchema.BlockStatus, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool BlockStatusSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.BlockStatus);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 55)]
		public bool? HasBlockedImages
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.HasBlockedImages);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool HasBlockedImagesSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.HasBlockedImages);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 56)]
		public BodyContentType TextBody
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BodyContentType>(ItemSchema.TextBody);
			}
			set
			{
				base.PropertyBag[ItemSchema.TextBody] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 57)]
		[XmlIgnore]
		public bool ContainsOnlyMandatoryProperties { get; set; }

		[IgnoreDataMember]
		[XmlElement]
		public IconIndexType IconIndex
		{
			get
			{
				if (!this.IconIndexSpecified)
				{
					return (IconIndexType)0;
				}
				return EnumUtilities.Parse<IconIndexType>(this.IconIndexString);
			}
			set
			{
				this.IconIndexString = EnumUtilities.ToString<IconIndexType>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "IconIndex", EmitDefaultValue = false, Order = 58)]
		public string IconIndexString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.IconIndex);
			}
			set
			{
				base.PropertyBag[ItemSchema.IconIndex] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IconIndexSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.IconIndex);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 59)]
		[XmlIgnore]
		public PropertyExistenceType PropertyExistence { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 60)]
		[XmlIgnore]
		public PropertyErrorType[] ErrorProperties
		{
			get
			{
				if (this.errorProperties == null)
				{
					return null;
				}
				return this.errorProperties.ToArray();
			}
			set
			{
				this.errorProperties = ((value != null) ? new List<PropertyErrorType>(value) : null);
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 61)]
		public ConversationType Conversation { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 62)]
		[XmlIgnore]
		public short? RichContent
		{
			get
			{
				return base.PropertyBag.GetNullableValue<short>(ItemSchema.RichContent);
			}
			set
			{
				base.PropertyBag.SetNullableValue<short>(ItemSchema.RichContent, value);
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 63)]
		[DateTimeString]
		public string ReceivedOrRenewTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.ReceivedOrRenewTime);
			}
			set
			{
				base.PropertyBag[ItemSchema.ReceivedOrRenewTime] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ReceivedOrRenewTimeSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.ReceivedOrRenewTime);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 64)]
		public string WorkingSetSourcePartition
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.WorkingSetSourcePartition);
			}
			set
			{
				base.PropertyBag[ItemSchema.WorkingSetSourcePartition] = value;
			}
		}

		[XmlIgnore]
		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 64)]
		public string RenewTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ItemSchema.RenewTime);
			}
			set
			{
				base.PropertyBag[ItemSchema.RenewTime] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool RenewTimeSpecified
		{
			get
			{
				return base.IsSet(ItemSchema.RenewTime);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 66)]
		[XmlIgnore]
		public bool? SupportsSideConversation
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ItemSchema.SupportsSideConversation);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 67)]
		public MimeContentType MimeContentUTF8
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<MimeContentType>(ItemSchema.MimeContentUTF8);
			}
			set
			{
				base.PropertyBag[ItemSchema.MimeContentUTF8] = value;
			}
		}

		internal void AddPropertyError(PropertyPath property, PropertyErrorCodeType errorCode)
		{
			if (this.errorProperties == null)
			{
				this.errorProperties = new List<PropertyErrorType>();
			}
			this.errorProperties.Add(new PropertyErrorType
			{
				PropertyPath = property,
				ErrorCode = errorCode
			});
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.Unknown;
			}
		}

		internal override void AddExtendedPropertyValue(ExtendedPropertyType extendedPropertyToAdd)
		{
			ExtendedPropertyType[] extendedProperty = this.ExtendedProperty;
			int num = (extendedProperty == null) ? 0 : extendedProperty.Length;
			ExtendedPropertyType[] array = new ExtendedPropertyType[num + 1];
			if (num > 0)
			{
				Array.Copy(extendedProperty, array, num);
			}
			array[num] = extendedPropertyToAdd;
			this.ExtendedProperty = array;
		}

		internal bool ContainsExtendedProperty(ExtendedPropertyUri extendedPropertyUri)
		{
			if (this.ExtendedProperty != null)
			{
				foreach (ExtendedPropertyType extendedPropertyType in this.ExtendedProperty)
				{
					if (ExtendedPropertyUri.AreEqual(extendedPropertyUri, extendedPropertyType.ExtendedFieldURI))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static LazyMember<Dictionary<StoreObjectType, Func<ItemType>>> createMethods = new LazyMember<Dictionary<StoreObjectType, Func<ItemType>>>(delegate()
		{
			Dictionary<StoreObjectType, Func<ItemType>> dictionary = new Dictionary<StoreObjectType, Func<ItemType>>();
			dictionary.Add(StoreObjectType.CalendarItem, () => new EwsCalendarItemType());
			dictionary.Add(StoreObjectType.CalendarItemOccurrence, () => new EwsCalendarItemType());
			dictionary.Add(StoreObjectType.Contact, () => new ContactItemType());
			dictionary.Add(StoreObjectType.DistributionList, () => new DistributionListType());
			dictionary.Add(StoreObjectType.MeetingCancellation, () => new MeetingCancellationMessageType());
			dictionary.Add(StoreObjectType.MeetingMessage, () => new MeetingMessageType());
			dictionary.Add(StoreObjectType.MeetingRequest, () => new MeetingRequestMessageType());
			dictionary.Add(StoreObjectType.MeetingResponse, () => new MeetingResponseMessageType());
			dictionary.Add(StoreObjectType.Message, () => new MessageType());
			dictionary.Add(StoreObjectType.Post, () => new PostItemType());
			dictionary.Add(StoreObjectType.Task, () => new TaskType());
			dictionary.Add(StoreObjectType.Unknown, () => new MessageType());
			return dictionary;
		});

		private List<PropertyErrorType> errorProperties;
	}
}
