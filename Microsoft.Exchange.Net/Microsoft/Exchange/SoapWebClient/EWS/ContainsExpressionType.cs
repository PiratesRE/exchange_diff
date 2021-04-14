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
	public class ContainsExpressionType : SearchExpressionType
	{
		[XmlElement("FieldURI", typeof(PathToUnindexedFieldType))]
		[XmlElement("ExtendedFieldURI", typeof(PathToExtendedFieldType))]
		[XmlElement("IndexedFieldURI", typeof(PathToIndexedFieldType))]
		public BasePathToElementType Item;

		public ConstantValueType Constant;

		[XmlAttribute]
		public ContainmentModeType ContainmentMode;

		[XmlIgnore]
		public bool ContainmentModeSpecified;

		[XmlAttribute]
		public ContainmentComparisonType ContainmentComparison;

		[XmlIgnore]
		public bool ContainmentComparisonSpecified;
	}
}
