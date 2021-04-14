using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[Serializable]
	public class ConversationType
	{
		public ItemIdType ConversationId;

		public string ConversationTopic;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueRecipients;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalUniqueRecipients;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueUnreadSenders;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalUniqueUnreadSenders;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueSenders;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalUniqueSenders;

		public DateTime LastDeliveryTime;

		[XmlIgnore]
		public bool LastDeliveryTimeSpecified;

		public DateTime GlobalLastDeliveryTime;

		[XmlIgnore]
		public bool GlobalLastDeliveryTimeSpecified;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalCategories;

		public FlagStatusType FlagStatus;

		[XmlIgnore]
		public bool FlagStatusSpecified;

		public FlagStatusType GlobalFlagStatus;

		[XmlIgnore]
		public bool GlobalFlagStatusSpecified;

		public bool HasAttachments;

		[XmlIgnore]
		public bool HasAttachmentsSpecified;

		public bool GlobalHasAttachments;

		[XmlIgnore]
		public bool GlobalHasAttachmentsSpecified;

		public int MessageCount;

		[XmlIgnore]
		public bool MessageCountSpecified;

		public int GlobalMessageCount;

		[XmlIgnore]
		public bool GlobalMessageCountSpecified;

		public int UnreadCount;

		[XmlIgnore]
		public bool UnreadCountSpecified;

		public int GlobalUnreadCount;

		[XmlIgnore]
		public bool GlobalUnreadCountSpecified;

		public int Size;

		[XmlIgnore]
		public bool SizeSpecified;

		public int GlobalSize;

		[XmlIgnore]
		public bool GlobalSizeSpecified;

		[XmlArrayItem("ItemClass", IsNullable = false)]
		public string[] ItemClasses;

		[XmlArrayItem("ItemClass", IsNullable = false)]
		public string[] GlobalItemClasses;

		public ImportanceChoicesType Importance;

		[XmlIgnore]
		public bool ImportanceSpecified;

		public ImportanceChoicesType GlobalImportance;

		[XmlIgnore]
		public bool GlobalImportanceSpecified;

		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), IsNullable = false)]
		public BaseItemIdType[] ItemIds;

		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemIdType), IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), IsNullable = false)]
		public BaseItemIdType[] GlobalItemIds;

		public DateTime LastModifiedTime;

		[XmlIgnore]
		public bool LastModifiedTimeSpecified;

		[XmlElement(DataType = "base64Binary")]
		public byte[] InstanceKey;

		public string Preview;

		public MailboxSearchLocationType MailboxScope;

		[XmlIgnore]
		public bool MailboxScopeSpecified;

		public IconIndexType IconIndex;

		[XmlIgnore]
		public bool IconIndexSpecified;

		public IconIndexType GlobalIconIndex;

		[XmlIgnore]
		public bool GlobalIconIndexSpecified;

		[XmlArrayItem("ItemId", typeof(ItemIdType), IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemIdType), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRangesType), IsNullable = false)]
		public BaseItemIdType[] DraftItemIds;

		public bool HasIrm;

		[XmlIgnore]
		public bool HasIrmSpecified;

		public bool GlobalHasIrm;

		[XmlIgnore]
		public bool GlobalHasIrmSpecified;
	}
}
