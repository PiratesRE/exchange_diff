using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class FileAttachmentType : AttachmentType
	{
		public bool IsContactPhoto;

		[XmlIgnore]
		public bool IsContactPhotoSpecified;

		[XmlElement(DataType = "base64Binary")]
		public byte[] Content;
	}
}
