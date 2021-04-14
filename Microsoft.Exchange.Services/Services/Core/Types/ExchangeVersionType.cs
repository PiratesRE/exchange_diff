using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ExchangeVersionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ExchangeVersionType
	{
		Exchange2007,
		Exchange2007_SP1,
		Exchange2009,
		Exchange2010,
		Exchange2010_SP1,
		Exchange2010_SP2,
		Exchange2012,
		Exchange2013,
		V2_1,
		V2_2,
		V2_3,
		V2_4,
		V2_5,
		V2_6,
		V2_7,
		Exchange2013_SP1,
		V2_8,
		V2_9,
		V2_10,
		V2_11,
		V2_12,
		V2_13,
		V2_14,
		V2_15,
		V2_16,
		V2_17,
		V2_18,
		V2_19,
		V2_20,
		V2_21,
		V2_22,
		V2_23
	}
}
