using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class ContactType : EntityType
	{
		public string PersonName;

		public string BusinessName;

		[XmlArrayItem("Phone", IsNullable = false)]
		public PhoneType[] PhoneNumbers;

		[XmlArrayItem("Url", IsNullable = false)]
		public string[] Urls;

		[XmlArrayItem("EmailAddress", IsNullable = false)]
		public string[] EmailAddresses;

		[XmlArrayItem("Address", IsNullable = false)]
		public string[] Addresses;

		public string ContactString;
	}
}
