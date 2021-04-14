using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(AndType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(OrType))]
	[KnownType(typeof(AndType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(OrType))]
	[Serializable]
	public abstract class MultipleOperandBooleanExpressionType : SearchExpressionType, INonLeafSearchExpressionType
	{
		[XmlElement("Contains", typeof(ContainsExpressionType))]
		[XmlElement("Or", typeof(OrType))]
		[XmlElement("IsEqualTo", typeof(IsEqualToType))]
		[XmlElement("Exists", typeof(ExistsType))]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("IsNotEqualTo", typeof(IsNotEqualToType))]
		[XmlElement("SearchExpression", typeof(SearchExpressionType))]
		[XmlElement("And", typeof(AndType))]
		[XmlElement("IsGreaterThan", typeof(IsGreaterThanType))]
		[XmlElement("IsGreaterThanOrEqualTo", typeof(IsGreaterThanOrEqualToType))]
		[XmlElement("Excludes", typeof(ExcludesType))]
		[XmlElement("Not", typeof(NotType))]
		[XmlElement("IsLessThan", typeof(IsLessThanType))]
		[XmlElement("IsLessThanOrEqualTo", typeof(IsLessThanOrEqualToType))]
		public SearchExpressionType[] Items { get; set; }
	}
}
