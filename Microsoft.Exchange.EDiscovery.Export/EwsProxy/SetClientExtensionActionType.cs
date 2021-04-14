using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SetClientExtensionActionType
	{
		public ClientExtensionType ClientExtension
		{
			get
			{
				return this.clientExtensionField;
			}
			set
			{
				this.clientExtensionField = value;
			}
		}

		[XmlAttribute]
		public SetClientExtensionActionIdType ActionId
		{
			get
			{
				return this.actionIdField;
			}
			set
			{
				this.actionIdField = value;
			}
		}

		[XmlAttribute]
		public string ExtensionId
		{
			get
			{
				return this.extensionIdField;
			}
			set
			{
				this.extensionIdField = value;
			}
		}

		private ClientExtensionType clientExtensionField;

		private SetClientExtensionActionIdType actionIdField;

		private string extensionIdField;
	}
}
