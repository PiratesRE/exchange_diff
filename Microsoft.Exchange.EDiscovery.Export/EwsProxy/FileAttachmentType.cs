using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class FileAttachmentType : AttachmentType
	{
		public bool IsContactPhoto
		{
			get
			{
				return this.isContactPhotoField;
			}
			set
			{
				this.isContactPhotoField = value;
			}
		}

		[XmlIgnore]
		public bool IsContactPhotoSpecified
		{
			get
			{
				return this.isContactPhotoFieldSpecified;
			}
			set
			{
				this.isContactPhotoFieldSpecified = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] Content
		{
			get
			{
				return this.contentField;
			}
			set
			{
				this.contentField = value;
			}
		}

		private bool isContactPhotoField;

		private bool isContactPhotoFieldSpecified;

		private byte[] contentField;
	}
}
