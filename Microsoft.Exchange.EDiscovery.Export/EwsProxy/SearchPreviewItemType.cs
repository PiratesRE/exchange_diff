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
	public class SearchPreviewItemType
	{
		public ItemIdType Id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		public PreviewItemMailboxType Mailbox
		{
			get
			{
				return this.mailboxField;
			}
			set
			{
				this.mailboxField = value;
			}
		}

		public ItemIdType ParentId
		{
			get
			{
				return this.parentIdField;
			}
			set
			{
				this.parentIdField = value;
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

		public string UniqueHash
		{
			get
			{
				return this.uniqueHashField;
			}
			set
			{
				this.uniqueHashField = value;
			}
		}

		public string SortValue
		{
			get
			{
				return this.sortValueField;
			}
			set
			{
				this.sortValueField = value;
			}
		}

		public string OwaLink
		{
			get
			{
				return this.owaLinkField;
			}
			set
			{
				this.owaLinkField = value;
			}
		}

		public string Sender
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

		[XmlArrayItem("SmtpAddress", IsNullable = false)]
		public string[] ToRecipients
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

		[XmlArrayItem("SmtpAddress", IsNullable = false)]
		public string[] CcRecipients
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

		[XmlArrayItem("SmtpAddress", IsNullable = false)]
		public string[] BccRecipients
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

		public DateTime CreatedTime
		{
			get
			{
				return this.createdTimeField;
			}
			set
			{
				this.createdTimeField = value;
			}
		}

		[XmlIgnore]
		public bool CreatedTimeSpecified
		{
			get
			{
				return this.createdTimeFieldSpecified;
			}
			set
			{
				this.createdTimeFieldSpecified = value;
			}
		}

		public DateTime ReceivedTime
		{
			get
			{
				return this.receivedTimeField;
			}
			set
			{
				this.receivedTimeField = value;
			}
		}

		[XmlIgnore]
		public bool ReceivedTimeSpecified
		{
			get
			{
				return this.receivedTimeFieldSpecified;
			}
			set
			{
				this.receivedTimeFieldSpecified = value;
			}
		}

		public DateTime SentTime
		{
			get
			{
				return this.sentTimeField;
			}
			set
			{
				this.sentTimeField = value;
			}
		}

		[XmlIgnore]
		public bool SentTimeSpecified
		{
			get
			{
				return this.sentTimeFieldSpecified;
			}
			set
			{
				this.sentTimeFieldSpecified = value;
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

		public long Size
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

		public bool Read
		{
			get
			{
				return this.readField;
			}
			set
			{
				this.readField = value;
			}
		}

		[XmlIgnore]
		public bool ReadSpecified
		{
			get
			{
				return this.readFieldSpecified;
			}
			set
			{
				this.readFieldSpecified = value;
			}
		}

		public bool HasAttachment
		{
			get
			{
				return this.hasAttachmentField;
			}
			set
			{
				this.hasAttachmentField = value;
			}
		}

		[XmlIgnore]
		public bool HasAttachmentSpecified
		{
			get
			{
				return this.hasAttachmentFieldSpecified;
			}
			set
			{
				this.hasAttachmentFieldSpecified = value;
			}
		}

		public NonEmptyArrayOfExtendedPropertyType ExtendedProperties
		{
			get
			{
				return this.extendedPropertiesField;
			}
			set
			{
				this.extendedPropertiesField = value;
			}
		}

		private ItemIdType idField;

		private PreviewItemMailboxType mailboxField;

		private ItemIdType parentIdField;

		private string itemClassField;

		private string uniqueHashField;

		private string sortValueField;

		private string owaLinkField;

		private string senderField;

		private string[] toRecipientsField;

		private string[] ccRecipientsField;

		private string[] bccRecipientsField;

		private DateTime createdTimeField;

		private bool createdTimeFieldSpecified;

		private DateTime receivedTimeField;

		private bool receivedTimeFieldSpecified;

		private DateTime sentTimeField;

		private bool sentTimeFieldSpecified;

		private string subjectField;

		private long sizeField;

		private bool sizeFieldSpecified;

		private string previewField;

		private ImportanceChoicesType importanceField;

		private bool importanceFieldSpecified;

		private bool readField;

		private bool readFieldSpecified;

		private bool hasAttachmentField;

		private bool hasAttachmentFieldSpecified;

		private NonEmptyArrayOfExtendedPropertyType extendedPropertiesField;
	}
}
