using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ConversationType
	{
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

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueRecipients
		{
			get
			{
				return this.uniqueRecipientsField;
			}
			set
			{
				this.uniqueRecipientsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalUniqueRecipients
		{
			get
			{
				return this.globalUniqueRecipientsField;
			}
			set
			{
				this.globalUniqueRecipientsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueUnreadSenders
		{
			get
			{
				return this.uniqueUnreadSendersField;
			}
			set
			{
				this.uniqueUnreadSendersField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalUniqueUnreadSenders
		{
			get
			{
				return this.globalUniqueUnreadSendersField;
			}
			set
			{
				this.globalUniqueUnreadSendersField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueSenders
		{
			get
			{
				return this.uniqueSendersField;
			}
			set
			{
				this.uniqueSendersField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalUniqueSenders
		{
			get
			{
				return this.globalUniqueSendersField;
			}
			set
			{
				this.globalUniqueSendersField = value;
			}
		}

		public DateTime LastDeliveryTime
		{
			get
			{
				return this.lastDeliveryTimeField;
			}
			set
			{
				this.lastDeliveryTimeField = value;
			}
		}

		[XmlIgnore]
		public bool LastDeliveryTimeSpecified
		{
			get
			{
				return this.lastDeliveryTimeFieldSpecified;
			}
			set
			{
				this.lastDeliveryTimeFieldSpecified = value;
			}
		}

		public DateTime GlobalLastDeliveryTime
		{
			get
			{
				return this.globalLastDeliveryTimeField;
			}
			set
			{
				this.globalLastDeliveryTimeField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalLastDeliveryTimeSpecified
		{
			get
			{
				return this.globalLastDeliveryTimeFieldSpecified;
			}
			set
			{
				this.globalLastDeliveryTimeFieldSpecified = value;
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

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalCategories
		{
			get
			{
				return this.globalCategoriesField;
			}
			set
			{
				this.globalCategoriesField = value;
			}
		}

		public FlagStatusType FlagStatus
		{
			get
			{
				return this.flagStatusField;
			}
			set
			{
				this.flagStatusField = value;
			}
		}

		[XmlIgnore]
		public bool FlagStatusSpecified
		{
			get
			{
				return this.flagStatusFieldSpecified;
			}
			set
			{
				this.flagStatusFieldSpecified = value;
			}
		}

		public FlagStatusType GlobalFlagStatus
		{
			get
			{
				return this.globalFlagStatusField;
			}
			set
			{
				this.globalFlagStatusField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalFlagStatusSpecified
		{
			get
			{
				return this.globalFlagStatusFieldSpecified;
			}
			set
			{
				this.globalFlagStatusFieldSpecified = value;
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

		public bool GlobalHasAttachments
		{
			get
			{
				return this.globalHasAttachmentsField;
			}
			set
			{
				this.globalHasAttachmentsField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalHasAttachmentsSpecified
		{
			get
			{
				return this.globalHasAttachmentsFieldSpecified;
			}
			set
			{
				this.globalHasAttachmentsFieldSpecified = value;
			}
		}

		public int MessageCount
		{
			get
			{
				return this.messageCountField;
			}
			set
			{
				this.messageCountField = value;
			}
		}

		[XmlIgnore]
		public bool MessageCountSpecified
		{
			get
			{
				return this.messageCountFieldSpecified;
			}
			set
			{
				this.messageCountFieldSpecified = value;
			}
		}

		public int GlobalMessageCount
		{
			get
			{
				return this.globalMessageCountField;
			}
			set
			{
				this.globalMessageCountField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalMessageCountSpecified
		{
			get
			{
				return this.globalMessageCountFieldSpecified;
			}
			set
			{
				this.globalMessageCountFieldSpecified = value;
			}
		}

		public int UnreadCount
		{
			get
			{
				return this.unreadCountField;
			}
			set
			{
				this.unreadCountField = value;
			}
		}

		[XmlIgnore]
		public bool UnreadCountSpecified
		{
			get
			{
				return this.unreadCountFieldSpecified;
			}
			set
			{
				this.unreadCountFieldSpecified = value;
			}
		}

		public int GlobalUnreadCount
		{
			get
			{
				return this.globalUnreadCountField;
			}
			set
			{
				this.globalUnreadCountField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalUnreadCountSpecified
		{
			get
			{
				return this.globalUnreadCountFieldSpecified;
			}
			set
			{
				this.globalUnreadCountFieldSpecified = value;
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

		public int GlobalSize
		{
			get
			{
				return this.globalSizeField;
			}
			set
			{
				this.globalSizeField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalSizeSpecified
		{
			get
			{
				return this.globalSizeFieldSpecified;
			}
			set
			{
				this.globalSizeFieldSpecified = value;
			}
		}

		[XmlArrayItem("ItemClass", IsNullable = false)]
		public string[] ItemClasses
		{
			get
			{
				return this.itemClassesField;
			}
			set
			{
				this.itemClassesField = value;
			}
		}

		[XmlArrayItem("ItemClass", IsNullable = false)]
		public string[] GlobalItemClasses
		{
			get
			{
				return this.globalItemClassesField;
			}
			set
			{
				this.globalItemClassesField = value;
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

		public ImportanceChoicesType GlobalImportance
		{
			get
			{
				return this.globalImportanceField;
			}
			set
			{
				this.globalImportanceField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalImportanceSpecified
		{
			get
			{
				return this.globalImportanceFieldSpecified;
			}
			set
			{
				this.globalImportanceFieldSpecified = value;
			}
		}

		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), IsNullable = false)]
		public BaseItemIdType[] ItemIds
		{
			get
			{
				return this.itemIdsField;
			}
			set
			{
				this.itemIdsField = value;
			}
		}

		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), IsNullable = false)]
		public BaseItemIdType[] GlobalItemIds
		{
			get
			{
				return this.globalItemIdsField;
			}
			set
			{
				this.globalItemIdsField = value;
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

		public MailboxSearchLocationType MailboxScope
		{
			get
			{
				return this.mailboxScopeField;
			}
			set
			{
				this.mailboxScopeField = value;
			}
		}

		[XmlIgnore]
		public bool MailboxScopeSpecified
		{
			get
			{
				return this.mailboxScopeFieldSpecified;
			}
			set
			{
				this.mailboxScopeFieldSpecified = value;
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

		public IconIndexType GlobalIconIndex
		{
			get
			{
				return this.globalIconIndexField;
			}
			set
			{
				this.globalIconIndexField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalIconIndexSpecified
		{
			get
			{
				return this.globalIconIndexFieldSpecified;
			}
			set
			{
				this.globalIconIndexFieldSpecified = value;
			}
		}

		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), IsNullable = false)]
		public BaseItemIdType[] DraftItemIds
		{
			get
			{
				return this.draftItemIdsField;
			}
			set
			{
				this.draftItemIdsField = value;
			}
		}

		public bool HasIrm
		{
			get
			{
				return this.hasIrmField;
			}
			set
			{
				this.hasIrmField = value;
			}
		}

		[XmlIgnore]
		public bool HasIrmSpecified
		{
			get
			{
				return this.hasIrmFieldSpecified;
			}
			set
			{
				this.hasIrmFieldSpecified = value;
			}
		}

		public bool GlobalHasIrm
		{
			get
			{
				return this.globalHasIrmField;
			}
			set
			{
				this.globalHasIrmField = value;
			}
		}

		[XmlIgnore]
		public bool GlobalHasIrmSpecified
		{
			get
			{
				return this.globalHasIrmFieldSpecified;
			}
			set
			{
				this.globalHasIrmFieldSpecified = value;
			}
		}

		private ItemIdType conversationIdField;

		private string conversationTopicField;

		private string[] uniqueRecipientsField;

		private string[] globalUniqueRecipientsField;

		private string[] uniqueUnreadSendersField;

		private string[] globalUniqueUnreadSendersField;

		private string[] uniqueSendersField;

		private string[] globalUniqueSendersField;

		private DateTime lastDeliveryTimeField;

		private bool lastDeliveryTimeFieldSpecified;

		private DateTime globalLastDeliveryTimeField;

		private bool globalLastDeliveryTimeFieldSpecified;

		private string[] categoriesField;

		private string[] globalCategoriesField;

		private FlagStatusType flagStatusField;

		private bool flagStatusFieldSpecified;

		private FlagStatusType globalFlagStatusField;

		private bool globalFlagStatusFieldSpecified;

		private bool hasAttachmentsField;

		private bool hasAttachmentsFieldSpecified;

		private bool globalHasAttachmentsField;

		private bool globalHasAttachmentsFieldSpecified;

		private int messageCountField;

		private bool messageCountFieldSpecified;

		private int globalMessageCountField;

		private bool globalMessageCountFieldSpecified;

		private int unreadCountField;

		private bool unreadCountFieldSpecified;

		private int globalUnreadCountField;

		private bool globalUnreadCountFieldSpecified;

		private int sizeField;

		private bool sizeFieldSpecified;

		private int globalSizeField;

		private bool globalSizeFieldSpecified;

		private string[] itemClassesField;

		private string[] globalItemClassesField;

		private ImportanceChoicesType importanceField;

		private bool importanceFieldSpecified;

		private ImportanceChoicesType globalImportanceField;

		private bool globalImportanceFieldSpecified;

		private BaseItemIdType[] itemIdsField;

		private BaseItemIdType[] globalItemIdsField;

		private DateTime lastModifiedTimeField;

		private bool lastModifiedTimeFieldSpecified;

		private byte[] instanceKeyField;

		private string previewField;

		private MailboxSearchLocationType mailboxScopeField;

		private bool mailboxScopeFieldSpecified;

		private IconIndexType iconIndexField;

		private bool iconIndexFieldSpecified;

		private IconIndexType globalIconIndexField;

		private bool globalIconIndexFieldSpecified;

		private BaseItemIdType[] draftItemIdsField;

		private bool hasIrmField;

		private bool hasIrmFieldSpecified;

		private bool globalHasIrmField;

		private bool globalHasIrmFieldSpecified;
	}
}
