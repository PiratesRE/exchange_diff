using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class MailTipsResponseMessageType : ResponseMessageType
	{
		public MailTips MailTips
		{
			get
			{
				return this.mailTipsField;
			}
			set
			{
				this.mailTipsField = value;
			}
		}

		private MailTips mailTipsField;
	}
}
