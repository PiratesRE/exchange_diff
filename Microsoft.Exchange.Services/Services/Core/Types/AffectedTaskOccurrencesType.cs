using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AffectedTaskOccurrencesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum AffectedTaskOccurrencesType
	{
		AllOccurrences,
		SpecifiedOccurrenceOnly
	}
}
