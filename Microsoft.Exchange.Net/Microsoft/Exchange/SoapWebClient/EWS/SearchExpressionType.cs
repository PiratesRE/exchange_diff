using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(MultipleOperandBooleanExpressionType))]
	[XmlInclude(typeof(OrType))]
	[XmlInclude(typeof(AndType))]
	[XmlInclude(typeof(NotType))]
	[XmlInclude(typeof(ContainsExpressionType))]
	[XmlInclude(typeof(ExcludesType))]
	[XmlInclude(typeof(TwoOperandExpressionType))]
	[XmlInclude(typeof(IsLessThanOrEqualToType))]
	[XmlInclude(typeof(IsLessThanType))]
	[XmlInclude(typeof(IsGreaterThanOrEqualToType))]
	[XmlInclude(typeof(IsGreaterThanType))]
	[XmlInclude(typeof(IsNotEqualToType))]
	[XmlInclude(typeof(IsEqualToType))]
	[XmlInclude(typeof(ExistsType))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public abstract class SearchExpressionType
	{
	}
}
