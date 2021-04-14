using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UMCDRGroupByType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum UMCDRGroupByType
	{
		[XmlEnum("Day")]
		Day,
		[XmlEnum("Month")]
		Month,
		[XmlEnum("Total")]
		Total
	}
}
