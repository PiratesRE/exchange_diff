using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class MailboxQueryType
	{
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

		[XmlArrayItem("MailboxSearchScope", IsNullable = false)]
		public MailboxSearchScopeType[] MailboxSearchScopes
		{
			get
			{
				return this.mailboxSearchScopesField;
			}
			set
			{
				this.mailboxSearchScopesField = value;
			}
		}

		private string queryField;

		private MailboxSearchScopeType[] mailboxSearchScopesField;
	}
}
