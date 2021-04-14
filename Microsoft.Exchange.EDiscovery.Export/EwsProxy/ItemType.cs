using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(DistributionListType))]
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
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(ContactItemType))]
	[XmlInclude(typeof(CalendarItemType))]
	[XmlInclude(typeof(MessageType))]
	[XmlInclude(typeof(MeetingMessageType))]
	[XmlInclude(typeof(MeetingCancellationMessageType))]
	[XmlInclude(typeof(PostItemType))]
	[XmlInclude(typeof(TaskType))]
	[Serializable]
	public class ItemType
	{
		public MimeContentType MimeContent
		{
			get
			{
				return this.mimeContentField;
			}
			set
			{
				this.mimeContentField = value;
			}
		}

		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		public FolderIdType ParentFolderId
		{
			get
			{
				return this.parentFolderIdField;
			}
			set
			{
				this.parentFolderIdField = value;
			}
		}

		public string ItemClass
		{
			get
			{
				return this.itemClassField;
			}
			set
			{
				this.itemClassField = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subjectField;
			}
			set
			{
				this.subjectField = value;
			}
		}

		public SensitivityChoicesType Sensitivity
		{
			get
			{
				return this.sensitivityField;
			}
			set
			{
				this.sensitivityField = value;
			}
		}

		[XmlIgnore]
		public bool SensitivitySpecified
		{
			get
			{
				return this.sensitivityFieldSpecified;
			}
			set
			{
				this.sensitivityFieldSpecified = value;
			}
		}

		public BodyType Body
		{
			get
			{
				return this.bodyField;
			}
			set
			{
				this.bodyField = value;
			}
		}

		[XmlArrayItem("ItemAttachment", typeof(ItemAttachmentType), IsNullable = false)]
		[XmlArrayItem("FileAttachment", typeof(FileAttachmentType), IsNullable = false)]
		public AttachmentType[] Attachments
		{
			get
			{
				return this.attachmentsField;
			}
			set
			{
				this.attachmentsField = value;
			}
		}

		public DateTime DateTimeReceived
		{
			get
			{
				return this.dateTimeReceivedField;
			}
			set
			{
				this.dateTimeReceivedField = value;
			}
		}

		[XmlIgnore]
		public bool DateTimeReceivedSpecified
		{
			get
			{
				return this.dateTimeReceivedFieldSpecified;
			}
			set
			{
				this.dateTimeReceivedFieldSpecified = value;
			}
		}

		public int Size
		{
			get
			{
				return this.sizeField;
			}
			set
			{
				this.sizeField = value;
			}
		}

		[XmlIgnore]
		public bool SizeSpecified
		{
			get
			{
				return this.sizeFieldSpecified;
			}
			set
			{
				this.sizeFieldSpecified = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories
		{
			get
			{
				return this.categoriesField;
			}
			set
			{
				this.categoriesField = value;
			}
		}

		public ImportanceChoicesType Importance
		{
			get
			{
				return this.importanceField;
			}
			set
			{
				this.importanceField = value;
			}
		}

		[XmlIgnore]
		public bool ImportanceSpecified
		{
			get
			{
				return this.importanceFieldSpecified;
			}
			set
			{
				this.importanceFieldSpecified = value;
			}
		}

		public string InReplyTo
		{
			get
			{
				return this.inReplyToField;
			}
			set
			{
				this.inReplyToField = value;
			}
		}

		public bool IsSubmitted
		{
			get
			{
				return this.isSubmittedField;
			}
			set
			{
				this.isSubmittedField = value;
			}
		}

		[XmlIgnore]
		public bool IsSubmittedSpecified
		{
			get
			{
				return this.isSubmittedFieldSpecified;
			}
			set
			{
				this.isSubmittedFieldSpecified = value;
			}
		}

		public bool IsDraft
		{
			get
			{
				return this.isDraftField;
			}
			set
			{
				this.isDraftField = value;
			}
		}

		[XmlIgnore]
		public bool IsDraftSpecified
		{
			get
			{
				return this.isDraftFieldSpecified;
			}
			set
			{
				this.isDraftFieldSpecified = value;
			}
		}

		public bool IsFromMe
		{
			get
			{
				return this.isFromMeField;
			}
			set
			{
				this.isFromMeField = value;
			}
		}

		[XmlIgnore]
		public bool IsFromMeSpecified
		{
			get
			{
				return this.isFromMeFieldSpecified;
			}
			set
			{
				this.isFromMeFieldSpecified = value;
			}
		}

		public bool IsResend
		{
			get
			{
				return this.isResendField;
			}
			set
			{
				this.isResendField = value;
			}
		}

		[XmlIgnore]
		public bool IsResendSpecified
		{
			get
			{
				return this.isResendFieldSpecified;
			}
			set
			{
				this.isResendFieldSpecified = value;
			}
		}

		public bool IsUnmodified
		{
			get
			{
				return this.isUnmodifiedField;
			}
			set
			{
				this.isUnmodifiedField = value;
			}
		}

		[XmlIgnore]
		public bool IsUnmodifiedSpecified
		{
			get
			{
				return this.isUnmodifiedFieldSpecified;
			}
			set
			{
				this.isUnmodifiedFieldSpecified = value;
			}
		}

		[XmlArrayItem("InternetMessageHeader", IsNullable = false)]
		public InternetHeaderType[] InternetMessageHeaders
		{
			get
			{
				return this.internetMessageHeadersField;
			}
			set
			{
				this.internetMessageHeadersField = value;
			}
		}

		public DateTime DateTimeSent
		{
			get
			{
				return this.dateTimeSentField;
			}
			set
			{
				this.dateTimeSentField = value;
			}
		}

		[XmlIgnore]
		public bool DateTimeSentSpecified
		{
			get
			{
				return this.dateTimeSentFieldSpecified;
			}
			set
			{
				this.dateTimeSentFieldSpecified = value;
			}
		}

		public DateTime DateTimeCreated
		{
			get
			{
				return this.dateTimeCreatedField;
			}
			set
			{
				this.dateTimeCreatedField = value;
			}
		}

		[XmlIgnore]
		public bool DateTimeCreatedSpecified
		{
			get
			{
				return this.dateTimeCreatedFieldSpecified;
			}
			set
			{
				this.dateTimeCreatedFieldSpecified = value;
			}
		}

		[XmlArrayItem("ForwardItem", typeof(ForwardItemType), IsNullable = false)]
		[XmlArrayItem("RemoveItem", typeof(RemoveItemType), IsNullable = false)]
		[XmlArrayItem("ReplyAllToItem", typeof(ReplyAllToItemType), IsNullable = false)]
		[XmlArrayItem("ReplyToItem", typeof(ReplyToItemType), IsNullable = false)]
		[XmlArrayItem("SuppressReadReceipt", typeof(SuppressReadReceiptType), IsNullable = false)]
		[XmlArrayItem("AcceptItem", typeof(AcceptItemType), IsNullable = false)]
		[XmlArrayItem("TentativelyAcceptItem", typeof(TentativelyAcceptItemType), IsNullable = false)]
		[XmlArrayItem("PostReplyItem", typeof(PostReplyItemType), IsNullable = false)]
		[XmlArrayItem("ProposeNewTime", typeof(ProposeNewTimeType), IsNullable = false)]
		[XmlArrayItem("AcceptSharingInvitation", typeof(AcceptSharingInvitationType), IsNullable = false)]
		[XmlArrayItem("AddItemToMyCalendar", typeof(AddItemToMyCalendarType), IsNullable = false)]
		[XmlArrayItem("CancelCalendarItem", typeof(CancelCalendarItemType), IsNullable = false)]
		[XmlArrayItem("DeclineItem", typeof(DeclineItemType), IsNullable = false)]
		public ResponseObjectType[] ResponseObjects
		{
			get
			{
				return this.responseObjectsField;
			}
			set
			{
				this.responseObjectsField = value;
			}
		}

		public DateTime ReminderDueBy
		{
			get
			{
				return this.reminderDueByField;
			}
			set
			{
				this.reminderDueByField = value;
			}
		}

		[XmlIgnore]
		public bool ReminderDueBySpecified
		{
			get
			{
				return this.reminderDueByFieldSpecified;
			}
			set
			{
				this.reminderDueByFieldSpecified = value;
			}
		}

		public bool ReminderIsSet
		{
			get
			{
				return this.reminderIsSetField;
			}
			set
			{
				this.reminderIsSetField = value;
			}
		}

		[XmlIgnore]
		public bool ReminderIsSetSpecified
		{
			get
			{
				return this.reminderIsSetFieldSpecified;
			}
			set
			{
				this.reminderIsSetFieldSpecified = value;
			}
		}

		public DateTime ReminderNextTime
		{
			get
			{
				return this.reminderNextTimeField;
			}
			set
			{
				this.reminderNextTimeField = value;
			}
		}

		[XmlIgnore]
		public bool ReminderNextTimeSpecified
		{
			get
			{
				return this.reminderNextTimeFieldSpecified;
			}
			set
			{
				this.reminderNextTimeFieldSpecified = value;
			}
		}

		public string ReminderMinutesBeforeStart
		{
			get
			{
				return this.reminderMinutesBeforeStartField;
			}
			set
			{
				this.reminderMinutesBeforeStartField = value;
			}
		}

		public string DisplayCc
		{
			get
			{
				return this.displayCcField;
			}
			set
			{
				this.displayCcField = value;
			}
		}

		public string DisplayTo
		{
			get
			{
				return this.displayToField;
			}
			set
			{
				this.displayToField = value;
			}
		}

		public bool HasAttachments
		{
			get
			{
				return this.hasAttachmentsField;
			}
			set
			{
				this.hasAttachmentsField = value;
			}
		}

		[XmlIgnore]
		public bool HasAttachmentsSpecified
		{
			get
			{
				return this.hasAttachmentsFieldSpecified;
			}
			set
			{
				this.hasAttachmentsFieldSpecified = value;
			}
		}

		[XmlElement("ExtendedProperty")]
		public ExtendedPropertyType[] ExtendedProperty
		{
			get
			{
				return this.extendedPropertyField;
			}
			set
			{
				this.extendedPropertyField = value;
			}
		}

		[XmlElement(DataType = "language")]
		public string Culture
		{
			get
			{
				return this.cultureField;
			}
			set
			{
				this.cultureField = value;
			}
		}

		public EffectiveRightsType EffectiveRights
		{
			get
			{
				return this.effectiveRightsField;
			}
			set
			{
				this.effectiveRightsField = value;
			}
		}

		public string LastModifiedName
		{
			get
			{
				return this.lastModifiedNameField;
			}
			set
			{
				this.lastModifiedNameField = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTimeField;
			}
			set
			{
				this.lastModifiedTimeField = value;
			}
		}

		[XmlIgnore]
		public bool LastModifiedTimeSpecified
		{
			get
			{
				return this.lastModifiedTimeFieldSpecified;
			}
			set
			{
				this.lastModifiedTimeFieldSpecified = value;
			}
		}

		public bool IsAssociated
		{
			get
			{
				return this.isAssociatedField;
			}
			set
			{
				this.isAssociatedField = value;
			}
		}

		[XmlIgnore]
		public bool IsAssociatedSpecified
		{
			get
			{
				return this.isAssociatedFieldSpecified;
			}
			set
			{
				this.isAssociatedFieldSpecified = value;
			}
		}

		public string WebClientReadFormQueryString
		{
			get
			{
				return this.webClientReadFormQueryStringField;
			}
			set
			{
				this.webClientReadFormQueryStringField = value;
			}
		}

		public string WebClientEditFormQueryString
		{
			get
			{
				return this.webClientEditFormQueryStringField;
			}
			set
			{
				this.webClientEditFormQueryStringField = value;
			}
		}

		public ItemIdType ConversationId
		{
			get
			{
				return this.conversationIdField;
			}
			set
			{
				this.conversationIdField = value;
			}
		}

		public BodyType UniqueBody
		{
			get
			{
				return this.uniqueBodyField;
			}
			set
			{
				this.uniqueBodyField = value;
			}
		}

		public FlagType Flag
		{
			get
			{
				return this.flagField;
			}
			set
			{
				this.flagField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] StoreEntryId
		{
			get
			{
				return this.storeEntryIdField;
			}
			set
			{
				this.storeEntryIdField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] InstanceKey
		{
			get
			{
				return this.instanceKeyField;
			}
			set
			{
				this.instanceKeyField = value;
			}
		}

		public BodyType NormalizedBody
		{
			get
			{
				return this.normalizedBodyField;
			}
			set
			{
				this.normalizedBodyField = value;
			}
		}

		public EntityExtractionResultType EntityExtractionResult
		{
			get
			{
				return this.entityExtractionResultField;
			}
			set
			{
				this.entityExtractionResultField = value;
			}
		}

		public RetentionTagType PolicyTag
		{
			get
			{
				return this.policyTagField;
			}
			set
			{
				this.policyTagField = value;
			}
		}

		public RetentionTagType ArchiveTag
		{
			get
			{
				return this.archiveTagField;
			}
			set
			{
				this.archiveTagField = value;
			}
		}

		public DateTime RetentionDate
		{
			get
			{
				return this.retentionDateField;
			}
			set
			{
				this.retentionDateField = value;
			}
		}

		[XmlIgnore]
		public bool RetentionDateSpecified
		{
			get
			{
				return this.retentionDateFieldSpecified;
			}
			set
			{
				this.retentionDateFieldSpecified = value;
			}
		}

		public string Preview
		{
			get
			{
				return this.previewField;
			}
			set
			{
				this.previewField = value;
			}
		}

		public RightsManagementLicenseDataType RightsManagementLicenseData
		{
			get
			{
				return this.rightsManagementLicenseDataField;
			}
			set
			{
				this.rightsManagementLicenseDataField = value;
			}
		}

		[XmlArrayItem("PredictedActionReason", IsNullable = false)]
		public PredictedActionReasonType[] PredictedActionReasons
		{
			get
			{
				return this.predictedActionReasonsField;
			}
			set
			{
				this.predictedActionReasonsField = value;
			}
		}

		public bool IsClutter
		{
			get
			{
				return this.isClutterField;
			}
			set
			{
				this.isClutterField = value;
			}
		}

		[XmlIgnore]
		public bool IsClutterSpecified
		{
			get
			{
				return this.isClutterFieldSpecified;
			}
			set
			{
				this.isClutterFieldSpecified = value;
			}
		}

		public bool BlockStatus
		{
			get
			{
				return this.blockStatusField;
			}
			set
			{
				this.blockStatusField = value;
			}
		}

		[XmlIgnore]
		public bool BlockStatusSpecified
		{
			get
			{
				return this.blockStatusFieldSpecified;
			}
			set
			{
				this.blockStatusFieldSpecified = value;
			}
		}

		public bool HasBlockedImages
		{
			get
			{
				return this.hasBlockedImagesField;
			}
			set
			{
				this.hasBlockedImagesField = value;
			}
		}

		[XmlIgnore]
		public bool HasBlockedImagesSpecified
		{
			get
			{
				return this.hasBlockedImagesFieldSpecified;
			}
			set
			{
				this.hasBlockedImagesFieldSpecified = value;
			}
		}

		public BodyType TextBody
		{
			get
			{
				return this.textBodyField;
			}
			set
			{
				this.textBodyField = value;
			}
		}

		public IconIndexType IconIndex
		{
			get
			{
				return this.iconIndexField;
			}
			set
			{
				this.iconIndexField = value;
			}
		}

		[XmlIgnore]
		public bool IconIndexSpecified
		{
			get
			{
				return this.iconIndexFieldSpecified;
			}
			set
			{
				this.iconIndexFieldSpecified = value;
			}
		}

		private MimeContentType mimeContentField;

		private ItemIdType itemIdField;

		private FolderIdType parentFolderIdField;

		private string itemClassField;

		private string subjectField;

		private SensitivityChoicesType sensitivityField;

		private bool sensitivityFieldSpecified;

		private BodyType bodyField;

		private AttachmentType[] attachmentsField;

		private DateTime dateTimeReceivedField;

		private bool dateTimeReceivedFieldSpecified;

		private int sizeField;

		private bool sizeFieldSpecified;

		private string[] categoriesField;

		private ImportanceChoicesType importanceField;

		private bool importanceFieldSpecified;

		private string inReplyToField;

		private bool isSubmittedField;

		private bool isSubmittedFieldSpecified;

		private bool isDraftField;

		private bool isDraftFieldSpecified;

		private bool isFromMeField;

		private bool isFromMeFieldSpecified;

		private bool isResendField;

		private bool isResendFieldSpecified;

		private bool isUnmodifiedField;

		private bool isUnmodifiedFieldSpecified;

		private InternetHeaderType[] internetMessageHeadersField;

		private DateTime dateTimeSentField;

		private bool dateTimeSentFieldSpecified;

		private DateTime dateTimeCreatedField;

		private bool dateTimeCreatedFieldSpecified;

		private ResponseObjectType[] responseObjectsField;

		private DateTime reminderDueByField;

		private bool reminderDueByFieldSpecified;

		private bool reminderIsSetField;

		private bool reminderIsSetFieldSpecified;

		private DateTime reminderNextTimeField;

		private bool reminderNextTimeFieldSpecified;

		private string reminderMinutesBeforeStartField;

		private string displayCcField;

		private string displayToField;

		private bool hasAttachmentsField;

		private bool hasAttachmentsFieldSpecified;

		private ExtendedPropertyType[] extendedPropertyField;

		private string cultureField;

		private EffectiveRightsType effectiveRightsField;

		private string lastModifiedNameField;

		private DateTime lastModifiedTimeField;

		private bool lastModifiedTimeFieldSpecified;

		private bool isAssociatedField;

		private bool isAssociatedFieldSpecified;

		private string webClientReadFormQueryStringField;

		private string webClientEditFormQueryStringField;

		private ItemIdType conversationIdField;

		private BodyType uniqueBodyField;

		private FlagType flagField;

		private byte[] storeEntryIdField;

		private byte[] instanceKeyField;

		private BodyType normalizedBodyField;

		private EntityExtractionResultType entityExtractionResultField;

		private RetentionTagType policyTagField;

		private RetentionTagType archiveTagField;

		private DateTime retentionDateField;

		private bool retentionDateFieldSpecified;

		private string previewField;

		private RightsManagementLicenseDataType rightsManagementLicenseDataField;

		private PredictedActionReasonType[] predictedActionReasonsField;

		private bool isClutterField;

		private bool isClutterFieldSpecified;

		private bool blockStatusField;

		private bool blockStatusFieldSpecified;

		private bool hasBlockedImagesField;

		private bool hasBlockedImagesFieldSpecified;

		private BodyType textBodyField;

		private IconIndexType iconIndexField;

		private bool iconIndexFieldSpecified;
	}
}
