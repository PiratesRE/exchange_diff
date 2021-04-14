using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class EmailAddressType : BaseEmailAddressType
	{
		public string Name;

		public string EmailAddress;

		public string RoutingType;

		public MailboxTypeType MailboxType;

		[XmlIgnore]
		public bool MailboxTypeSpecified;

		public ItemIdType ItemId;

		public string OriginalDisplayName;
	}
}
