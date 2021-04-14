using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class MasterMailboxType
	{
		public string MailboxType;

		public string Alias;

		public string DisplayName;

		public string SmtpAddress;

		public ModernGroupTypeType GroupType;

		[XmlIgnore]
		public bool GroupTypeSpecified;

		public string Description;

		public string Photo;

		public string SharePointUrl;

		public string InboxUrl;

		public string CalendarUrl;

		public string DomainController;
	}
}
