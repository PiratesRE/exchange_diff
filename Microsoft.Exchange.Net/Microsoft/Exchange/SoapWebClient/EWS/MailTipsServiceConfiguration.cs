using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MailTipsServiceConfiguration : ServiceConfiguration
	{
		public bool MailTipsEnabled;

		public int MaxRecipientsPerGetMailTipsRequest;

		public int MaxMessageSize;

		public int LargeAudienceThreshold;

		public bool ShowExternalRecipientCount;

		[XmlArrayItem("Domain", IsNullable = false)]
		public SmtpDomain[] InternalDomains;

		public bool PolicyTipsEnabled;

		public int LargeAudienceCap;
	}
}
