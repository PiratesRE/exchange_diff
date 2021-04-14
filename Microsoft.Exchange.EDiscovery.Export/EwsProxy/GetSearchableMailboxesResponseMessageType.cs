using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetSearchableMailboxesResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("SearchableMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public SearchableMailboxType[] SearchableMailboxes
		{
			get
			{
				return this.searchableMailboxesField;
			}
			set
			{
				this.searchableMailboxesField = value;
			}
		}

		[XmlArrayItem("FailedMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FailedSearchMailboxType[] FailedMailboxes
		{
			get
			{
				return this.failedMailboxesField;
			}
			set
			{
				this.failedMailboxesField = value;
			}
		}

		private SearchableMailboxType[] searchableMailboxesField;

		private FailedSearchMailboxType[] failedMailboxesField;
	}
}
