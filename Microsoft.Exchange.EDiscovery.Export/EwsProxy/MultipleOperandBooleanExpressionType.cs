using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(OrType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(AndType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public abstract class MultipleOperandBooleanExpressionType : SearchExpressionType
	{
		[XmlElement("Excludes", typeof(ExcludesType))]
		[XmlElement("And", typeof(AndType))]
		[XmlElement("Contains", typeof(ContainsExpressionType))]
		[XmlElement("Exists", typeof(ExistsType))]
		[XmlElement("IsEqualTo", typeof(IsEqualToType))]
		[XmlElement("IsLessThanOrEqualTo", typeof(IsLessThanOrEqualToType))]
		[XmlElement("IsGreaterThan", typeof(IsGreaterThanType))]
		[XmlElement("Not", typeof(NotType))]
		[XmlElement("Or", typeof(OrType))]
		[XmlElement("IsGreaterThanOrEqualTo", typeof(IsGreaterThanOrEqualToType))]
		[XmlElement("SearchExpression", typeof(SearchExpressionType))]
		[XmlElement("IsLessThan", typeof(IsLessThanType))]
		[XmlElement("IsNotEqualTo", typeof(IsNotEqualToType))]
		public SearchExpressionType[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		private SearchExpressionType[] itemsField;
	}
}
