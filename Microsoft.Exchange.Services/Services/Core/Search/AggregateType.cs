using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "AggregateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum AggregateType
	{
		[XmlEnum(Name = "Minimum")]
		Minimum,
		[XmlEnum(Name = "Maximum")]
		Maximum
	}
}
