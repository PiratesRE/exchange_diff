using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[XmlType(AnonymousType = true)]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[Serializable]
	public enum TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacityCodingScheme
	{
		GsmDefault,
		Unicode,
		UsAscii,
		Ia5,
		Iso_8859_1,
		Iso_8859_8,
		ShiftJis,
		EucKr
	}
}
