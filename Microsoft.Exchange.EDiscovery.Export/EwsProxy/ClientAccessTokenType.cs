using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class ClientAccessTokenType
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

		public string TokenValue
		{
			get
			{
				return this.tokenValueField;
			}
			set
			{
				this.tokenValueField = value;
			}
		}

		[XmlElement(DataType = "integer")]
		public string TTL
		{
			get
			{
				return this.tTLField;
			}
			set
			{
				this.tTLField = value;
			}
		}

		private string idField;

		private ClientAccessTokenTypeType tokenTypeField;

		private string tokenValueField;

		private string tTLField;
	}
}
