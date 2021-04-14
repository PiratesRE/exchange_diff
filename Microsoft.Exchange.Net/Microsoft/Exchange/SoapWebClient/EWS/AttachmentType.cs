using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlInclude(typeof(ReferenceAttachmentType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlInclude(typeof(FileAttachmentType))]
	[XmlInclude(typeof(ItemAttachmentType))]
	[Serializable]
	public class AttachmentType
	{
		public AttachmentIdType AttachmentId;

		public string Name;

		public string ContentType;

		public string ContentId;

		public string ContentLocation;

		public int Size;

		[XmlIgnore]
		public bool SizeSpecified;

		public DateTime LastModifiedTime;

		[XmlIgnore]
		public bool LastModifiedTimeSpecified;

		public bool IsInline;

		[XmlIgnore]
		public bool IsInlineSpecified;
	}
}
