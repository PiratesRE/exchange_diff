using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlRoot(ElementName = "DateTimePrecision", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlType(TypeName = "DateTimePrecisionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum DateTimePrecision
	{
		Seconds,
		Milliseconds
	}
}
