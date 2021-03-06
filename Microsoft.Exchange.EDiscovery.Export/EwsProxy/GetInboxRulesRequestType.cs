using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetInboxRulesRequestType : BaseRequestType
	{
		public string MailboxSmtpAddress
		{
			get
			{
				return this.mailboxSmtpAddressField;
			}
			set
			{
				this.mailboxSmtpAddressField = value;
			}
		}

		private string mailboxSmtpAddressField;
	}
}
