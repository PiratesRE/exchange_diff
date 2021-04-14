using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ItemResponseShapeType
	{
		public DefaultShapeNamesType BaseShape;

		public bool IncludeMimeContent;

		[XmlIgnore]
		public bool IncludeMimeContentSpecified;

		public BodyTypeResponseType BodyType;

		[XmlIgnore]
		public bool BodyTypeSpecified;

		public BodyTypeResponseType UniqueBodyType;

		[XmlIgnore]
		public bool UniqueBodyTypeSpecified;

		public BodyTypeResponseType NormalizedBodyType;

		[XmlIgnore]
		public bool NormalizedBodyTypeSpecified;

		public bool FilterHtmlContent;

		[XmlIgnore]
		public bool FilterHtmlContentSpecified;

		public bool ConvertHtmlCodePageToUTF8;

		[XmlIgnore]
		public bool ConvertHtmlCodePageToUTF8Specified;

		public string InlineImageUrlTemplate;

		public bool BlockExternalImages;

		[XmlIgnore]
		public bool BlockExternalImagesSpecified;

		public bool AddBlankTargetToLinks;

		[XmlIgnore]
		public bool AddBlankTargetToLinksSpecified;

		public int MaximumBodySize;

		[XmlIgnore]
		public bool MaximumBodySizeSpecified;

		[XmlArrayItem("Path", IsNullable = false)]
		[XmlArrayItem("ExtendedFieldURI", typeof(PathToExtendedFieldType), IsNullable = false)]
		[XmlArrayItem("IndexedFieldURI", typeof(PathToIndexedFieldType), IsNullable = false)]
		[XmlArrayItem("FieldURI", typeof(PathToUnindexedFieldType), IsNullable = false)]
		public BasePathToElementType[] AdditionalProperties;
	}
}
