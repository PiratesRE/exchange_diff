using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SearchMailboxesResponseMessageType : ResponseMessageType
	{
		public SearchMailboxesResultType SearchMailboxesResult
		{
			get
			{
				return this.searchMailboxesResultField;
			}
			set
			{
				this.searchMailboxesResultField = value;
			}
		}

		private SearchMailboxesResultType searchMailboxesResultField;
	}
}
