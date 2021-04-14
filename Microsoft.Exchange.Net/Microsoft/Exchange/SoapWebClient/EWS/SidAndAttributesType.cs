using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[CLSCompliant(false)]
	[Serializable]
	public class SidAndAttributesType
	{
		public string SecurityIdentifier;

		[XmlAttribute]
		public uint Attributes;
	}
}
