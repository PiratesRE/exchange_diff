using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetMailTipsType : BaseRequestType
	{
		public EmailAddressType SendingAs
		{
			get
			{
				return this.sendingAsField;
			}
			set
			{
				this.sendingAsField = value;
			}
		}

		[XmlArrayItem("Mailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public EmailAddressType[] Recipients
		{
			get
			{
				return this.recipientsField;
			}
			set
			{
				this.recipientsField = value;
			}
		}

		public MailTipTypes MailTipsRequested
		{
			get
			{
				return this.mailTipsRequestedField;
			}
			set
			{
				this.mailTipsRequestedField = value;
			}
		}

		private EmailAddressType sendingAsField;

		private EmailAddressType[] recipientsField;

		private MailTipTypes mailTipsRequestedField;
	}
}
