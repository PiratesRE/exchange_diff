using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PhysicalAddressDictionaryEntryType
	{
		public string Street;

		public string City;

		public string State;

		public string CountryOrRegion;

		public string PostalCode;

		[XmlAttribute]
		public PhysicalAddressKeyType Key;
	}
}
