using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class SearchPreviewItemType
	{
		public ItemIdType Id;

		public PreviewItemMailboxType Mailbox;

		public ItemIdType ParentId;

		public string ItemClass;

		public string UniqueHash;

		public string SortValue;

		public string OwaLink;

		public string Sender;

		[XmlArrayItem("SmtpAddress", IsNullable = false)]
		public string[] ToRecipients;

		[XmlArrayItem("SmtpAddress", IsNullable = false)]
		public string[] CcRecipients;

		[XmlArrayItem("SmtpAddress", IsNullable = false)]
		public string[] BccRecipients;

		public DateTime CreatedTime;

		[XmlIgnore]
		public bool CreatedTimeSpecified;

		public DateTime ReceivedTime;

		[XmlIgnore]
		public bool ReceivedTimeSpecified;

		public DateTime SentTime;

		[XmlIgnore]
		public bool SentTimeSpecified;

		public string Subject;

		public long Size;

		[XmlIgnore]
		public bool SizeSpecified;

		public string Preview;

		public ImportanceChoicesType Importance;

		[XmlIgnore]
		public bool ImportanceSpecified;

		public bool Read;

		[XmlIgnore]
		public bool ReadSpecified;

		public bool HasAttachment;

		[XmlIgnore]
		public bool HasAttachmentSpecified;

		public NonEmptyArrayOfExtendedPropertyType ExtendedProperties;
	}
}
