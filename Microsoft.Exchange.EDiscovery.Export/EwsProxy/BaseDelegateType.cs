using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(UpdateDelegateType))]
	[XmlInclude(typeof(RemoveDelegateType))]
	[XmlInclude(typeof(AddDelegateType))]
	[XmlInclude(typeof(GetDelegateType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public abstract class BaseDelegateType : BaseRequestType
	{
		public EmailAddressType Mailbox
		{
			get
			{
				return this.mailboxField;
			}
			set
			{
				this.mailboxField = value;
			}
		}

		private EmailAddressType mailboxField;
	}
}
