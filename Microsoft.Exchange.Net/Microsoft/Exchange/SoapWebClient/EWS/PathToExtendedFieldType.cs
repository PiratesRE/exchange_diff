using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class PathToExtendedFieldType : BasePathToElementType
	{
		[XmlAttribute]
		public DistinguishedPropertySetType DistinguishedPropertySetId;

		[XmlIgnore]
		public bool DistinguishedPropertySetIdSpecified;

		[XmlAttribute]
		public string PropertySetId;

		[XmlAttribute]
		public string PropertyTag;

		[XmlAttribute]
		public string PropertyName;

		[XmlAttribute]
		public int PropertyId;

		[XmlIgnore]
		public bool PropertyIdSpecified;

		[XmlAttribute]
		public MapiPropertyTypeType PropertyType;
	}
}
