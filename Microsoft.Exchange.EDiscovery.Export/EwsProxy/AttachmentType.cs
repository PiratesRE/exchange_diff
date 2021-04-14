using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlInclude(typeof(ItemAttachmentType))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(ReferenceAttachmentType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(FileAttachmentType))]
	[Serializable]
	public class AttachmentType
	{
		public AttachmentIdType AttachmentId
		{
			get
			{
				return this.attachmentIdField;
			}
			set
			{
				this.attachmentIdField = value;
			}
		}

		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this.contentTypeField;
			}
			set
			{
				this.contentTypeField = value;
			}
		}

		public string ContentId
		{
			get
			{
				return this.contentIdField;
			}
			set
			{
				this.contentIdField = value;
			}
		}

		public string ContentLocation
		{
			get
			{
				return this.contentLocationField;
			}
			set
			{
				this.contentLocationField = value;
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

		public bool IsInline
		{
			get
			{
				return this.isInlineField;
			}
			set
			{
				this.isInlineField = value;
			}
		}

		[XmlIgnore]
		public bool IsInlineSpecified
		{
			get
			{
				return this.isInlineFieldSpecified;
			}
			set
			{
				this.isInlineFieldSpecified = value;
			}
		}

		private AttachmentIdType attachmentIdField;

		private string nameField;

		private string contentTypeField;

		private string contentIdField;

		private string contentLocationField;

		private int sizeField;

		private bool sizeFieldSpecified;

		private DateTime lastModifiedTimeField;

		private bool lastModifiedTimeFieldSpecified;

		private bool isInlineField;

		private bool isInlineFieldSpecified;
	}
}
