using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "SearchPreviewItem", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SearchPreviewItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SearchPreviewItem
	{
		[XmlElement("Id")]
		[DataMember(Name = "Id", IsRequired = true)]
		public ItemId Id { get; set; }

		[XmlElement("Mailbox")]
		[DataMember(Name = "Mailbox", IsRequired = true)]
		public PreviewItemMailbox Mailbox { get; set; }

		[XmlElement("ParentId")]
		[DataMember(Name = "ParentId", IsRequired = true)]
		public ItemId ParentId { get; set; }

		[DataMember(Name = "ItemClass", IsRequired = false)]
		[XmlElement("ItemClass")]
		public string ItemClass { get; set; }

		[DataMember(Name = "UniqueHash", IsRequired = false)]
		[XmlElement("UniqueHash")]
		public string UniqueHash { get; set; }

		[XmlElement("SortValue")]
		[DataMember(Name = "SortValue", IsRequired = false)]
		public string SortValue { get; set; }

		[DataMember(Name = "OwaLink", IsRequired = false)]
		[XmlElement("OwaLink")]
		public string OwaLink { get; set; }

		[DataMember(Name = "Sender", IsRequired = false)]
		[XmlElement("Sender")]
		public string Sender { get; set; }

		[XmlArrayItem(ElementName = "SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(Name = "ToRecipients", IsRequired = false)]
		[XmlArray(ElementName = "ToRecipients", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string[] ToRecipients { get; set; }

		[DataMember(Name = "CcRecipients", IsRequired = false)]
		[XmlArray(ElementName = "CcRecipients", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] CcRecipients { get; set; }

		[XmlArray(ElementName = "BccRecipients", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(Name = "BccRecipients", IsRequired = false)]
		public string[] BccRecipients { get; set; }

		[DataMember(Name = "CreatedTime", EmitDefaultValue = false)]
		[XmlElement("CreatedTime")]
		public string CreatedTime { get; set; }

		[DataMember(Name = "ReceivedTime", EmitDefaultValue = false)]
		[XmlElement("ReceivedTime")]
		public string ReceivedTime { get; set; }

		[DataMember(Name = "SentTime", EmitDefaultValue = false)]
		[XmlElement("SentTime")]
		public string SentTime { get; set; }

		[DataMember(Name = "Subject", IsRequired = false)]
		[XmlElement("Subject")]
		public string Subject { get; set; }

		[XmlElement("Size")]
		[DataMember(Name = "Size", EmitDefaultValue = true, IsRequired = false)]
		public ulong? Size { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool SizeSpecified
		{
			get
			{
				return this.Size != null && this.Size != null;
			}
			set
			{
			}
		}

		[DataMember(Name = "Preview", IsRequired = false)]
		[XmlElement("Preview")]
		public string Preview { get; set; }

		[XmlElement("Importance")]
		[DataMember(Name = "Importance", IsRequired = false)]
		public string Importance { get; set; }

		[XmlElement("Read")]
		[DataMember(Name = "Read", EmitDefaultValue = false, IsRequired = false)]
		public bool? Read { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ReadSpecified
		{
			get
			{
				return this.Read != null && this.Read != null;
			}
			set
			{
			}
		}

		[DataMember(Name = "HasAttachment", EmitDefaultValue = false, IsRequired = false)]
		[XmlElement("HasAttachment")]
		public bool? HasAttachment { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool HasAttachmentSpecified
		{
			get
			{
				return this.HasAttachment != null && this.HasAttachment != null;
			}
			set
			{
			}
		}

		[XmlArrayItem(ElementName = "ExtendedProperty", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ExtendedPropertyType))]
		[XmlArray(ElementName = "ExtendedProperties", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "ExtendedProperties", EmitDefaultValue = false)]
		public ExtendedPropertyType[] ExtendedProperties { get; set; }
	}
}
