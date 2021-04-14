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
	public class NotType : SearchExpressionType
	{
		[XmlElement("IsLessThan", typeof(IsLessThanType))]
		[XmlElement("Contains", typeof(ContainsExpressionType))]
		[XmlElement("IsGreaterThanOrEqualTo", typeof(IsGreaterThanOrEqualToType))]
		[XmlElement("And", typeof(AndType))]
		[XmlElement("IsLessThanOrEqualTo", typeof(IsLessThanOrEqualToType))]
		[XmlElement("IsNotEqualTo", typeof(IsNotEqualToType))]
		[XmlElement("Not", typeof(NotType))]
		[XmlElement("Or", typeof(OrType))]
		[XmlElement("IsGreaterThan", typeof(IsGreaterThanType))]
		[XmlElement("IsEqualTo", typeof(IsEqualToType))]
		[XmlElement("Exists", typeof(ExistsType))]
		[XmlElement("SearchExpression", typeof(SearchExpressionType))]
		[XmlElement("Excludes", typeof(ExcludesType))]
		public SearchExpressionType Item;
	}
}
