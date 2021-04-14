using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class MailboxHoldResultType
	{
		public string HoldId
		{
			get
			{
				return this.holdIdField;
			}
			set
			{
				this.holdIdField = value;
			}
		}

		public string Query
		{
			get
			{
				return this.queryField;
			}
			set
			{
				this.queryField = value;
			}
		}

		[XmlArrayItem("MailboxHoldStatus", IsNullable = false)]
		public MailboxHoldStatusType[] MailboxHoldStatuses
		{
			get
			{
				return this.mailboxHoldStatusesField;
			}
			set
			{
				this.mailboxHoldStatusesField = value;
			}
		}

		private string holdIdField;

		private string queryField;

		private MailboxHoldStatusType[] mailboxHoldStatusesField;
	}
}
