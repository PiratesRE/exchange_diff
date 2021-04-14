using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(DistributionListType))]
	[XmlInclude(typeof(PostItemType))]
	[XmlInclude(typeof(TaskType))]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(CalendarItemType))]
	[XmlInclude(typeof(MessageType))]
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
	[XmlInclude(typeof(ContactItemType))]
	[DesignerCategory("code")]
	[Serializable]
	public class ItemType
	{
		public MimeContentType MimeContent;

		public ItemIdType ItemId;

		public FolderIdType ParentFolderId;

		public string ItemClass;

		public string Subject;

		public SensitivityChoicesType Sensitivity;

		[XmlIgnore]
		public bool SensitivitySpecified;

		public BodyType Body;

		[XmlArrayItem("FileAttachment", typeof(FileAttachmentType), IsNullable = false)]
		[XmlArrayItem("ItemAttachment", typeof(ItemAttachmentType), IsNullable = false)]
		public AttachmentType[] Attachments;

		public DateTime DateTimeReceived;

		[XmlIgnore]
		public bool DateTimeReceivedSpecified;

		public int Size;

		[XmlIgnore]
		public bool SizeSpecified;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories;

		public ImportanceChoicesType Importance;

		[XmlIgnore]
		public bool ImportanceSpecified;

		public string InReplyTo;

		public bool IsSubmitted;

		[XmlIgnore]
		public bool IsSubmittedSpecified;

		public bool IsDraft;

		[XmlIgnore]
		public bool IsDraftSpecified;

		public bool IsFromMe;

		[XmlIgnore]
		public bool IsFromMeSpecified;

		public bool IsResend;

		[XmlIgnore]
		public bool IsResendSpecified;

		public bool IsUnmodified;

		[XmlIgnore]
		public bool IsUnmodifiedSpecified;

		[XmlArrayItem("InternetMessageHeader", IsNullable = false)]
		public InternetHeaderType[] InternetMessageHeaders;

		public DateTime DateTimeSent;

		[XmlIgnore]
		public bool DateTimeSentSpecified;

		public DateTime DateTimeCreated;

		[XmlIgnore]
		public bool DateTimeCreatedSpecified;

		[XmlArrayItem("AcceptItem", typeof(AcceptItemType), IsNullable = false)]
		[XmlArrayItem("CancelCalendarItem", typeof(CancelCalendarItemType), IsNullable = false)]
		[XmlArrayItem("TentativelyAcceptItem", typeof(TentativelyAcceptItemType), IsNullable = false)]
		[XmlArrayItem("DeclineItem", typeof(DeclineItemType), IsNullable = false)]
		[XmlArrayItem("SuppressReadReceipt", typeof(SuppressReadReceiptType), IsNullable = false)]
		[XmlArrayItem("AcceptSharingInvitation", typeof(AcceptSharingInvitationType), IsNullable = false)]
		[XmlArrayItem("AddItemToMyCalendar", typeof(AddItemToMyCalendarType), IsNullable = false)]
		[XmlArrayItem("PostReplyItem", typeof(PostReplyItemType), IsNullable = false)]
		[XmlArrayItem("ProposeNewTime", typeof(ProposeNewTimeType), IsNullable = false)]
		[XmlArrayItem("ReplyToItem", typeof(ReplyToItemType), IsNullable = false)]
		[XmlArrayItem("ForwardItem", typeof(ForwardItemType), IsNullable = false)]
		[XmlArrayItem("RemoveItem", typeof(RemoveItemType), IsNullable = false)]
		[XmlArrayItem("ReplyAllToItem", typeof(ReplyAllToItemType), IsNullable = false)]
		public ResponseObjectType[] ResponseObjects;

		public DateTime ReminderDueBy;

		[XmlIgnore]
		public bool ReminderDueBySpecified;

		public bool ReminderIsSet;

		[XmlIgnore]
		public bool ReminderIsSetSpecified;

		public DateTime ReminderNextTime;

		[XmlIgnore]
		public bool ReminderNextTimeSpecified;

		public string ReminderMinutesBeforeStart;

		public string DisplayCc;

		public string DisplayTo;

		public bool HasAttachments;

		[XmlIgnore]
		public bool HasAttachmentsSpecified;

		[XmlElement("ExtendedProperty")]
		public ExtendedPropertyType[] ExtendedProperty;

		[XmlElement(DataType = "language")]
		public string Culture;

		public EffectiveRightsType EffectiveRights;

		public string LastModifiedName;

		public DateTime LastModifiedTime;

		[XmlIgnore]
		public bool LastModifiedTimeSpecified;

		public bool IsAssociated;

		[XmlIgnore]
		public bool IsAssociatedSpecified;

		public string WebClientReadFormQueryString;

		public string WebClientEditFormQueryString;

		public ItemIdType ConversationId;

		public BodyType UniqueBody;

		public FlagType Flag;

		[XmlElement(DataType = "base64Binary")]
		public byte[] StoreEntryId;

		[XmlElement(DataType = "base64Binary")]
		public byte[] InstanceKey;

		public BodyType NormalizedBody;

		public EntityExtractionResultType EntityExtractionResult;

		public RetentionTagType PolicyTag;

		public RetentionTagType ArchiveTag;

		public DateTime RetentionDate;

		[XmlIgnore]
		public bool RetentionDateSpecified;

		public string Preview;

		public RightsManagementLicenseDataType RightsManagementLicenseData;

		[XmlArrayItem("PredictedActionReason", IsNullable = false)]
		public PredictedActionReasonType[] PredictedActionReasons;

		public bool IsClutter;

		[XmlIgnore]
		public bool IsClutterSpecified;

		public bool BlockStatus;

		[XmlIgnore]
		public bool BlockStatusSpecified;

		public bool HasBlockedImages;

		[XmlIgnore]
		public bool HasBlockedImagesSpecified;

		public BodyType TextBody;

		public IconIndexType IconIndex;

		[XmlIgnore]
		public bool IconIndexSpecified;
	}
}
