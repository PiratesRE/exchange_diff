using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class ClientAccessTokenRequestType
	{
		public string Id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		public ClientAccessTokenTypeType TokenType
		{
			get
			{
				return this.tokenTypeField;
			}
			set
			{
				this.tokenTypeField = value;
			}
		}

		public string Scope
		{
			get
			{
				return this.scopeField;
			}
			set
			{
				this.scopeField = value;
			}
		}

		private string idField;

		private ClientAccessTokenTypeType tokenTypeField;

		private string scopeField;
	}
}
