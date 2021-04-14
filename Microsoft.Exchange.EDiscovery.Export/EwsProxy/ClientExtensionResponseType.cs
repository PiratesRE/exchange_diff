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
	public class ClientExtensionResponseType : ResponseMessageType
	{
		[XmlArrayItem("ClientExtension", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ClientExtensionType[] ClientExtensions
		{
			get
			{
				return this.clientExtensionsField;
			}
			set
			{
				this.clientExtensionsField = value;
			}
		}

		public string RawMasterTableXml
		{
			get
			{
				return this.rawMasterTableXmlField;
			}
			set
			{
				this.rawMasterTableXmlField = value;
			}
		}

		private ClientExtensionType[] clientExtensionsField;

		private string rawMasterTableXmlField;
	}
}
