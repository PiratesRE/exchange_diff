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
	public class AttachmentResponseShapeType
	{
		public bool IncludeMimeContent;

		[XmlIgnore]
		public bool IncludeMimeContentSpecified;

		public BodyTypeResponseType BodyType;

		[XmlIgnore]
		public bool BodyTypeSpecified;

		public bool FilterHtmlContent;

		[XmlIgnore]
		public bool FilterHtmlContentSpecified;

		[XmlArrayItem("Path", IsNullable = false)]
		[XmlArrayItem("IndexedFieldURI", typeof(PathToIndexedFieldType), IsNullable = false)]
		[XmlArrayItem("FieldURI", typeof(PathToUnindexedFieldType), IsNullable = false)]
		[XmlArrayItem("ExtendedFieldURI", typeof(PathToExtendedFieldType), IsNullable = false)]
		public BasePathToElementType[] AdditionalProperties;
	}
}
