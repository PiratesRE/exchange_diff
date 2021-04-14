using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
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
