using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetClientIntentType : BaseRequestType
	{
		public string GlobalObjectId
		{
			get
			{
				return this.globalObjectIdField;
			}
			set
			{
				this.globalObjectIdField = value;
			}
		}

		public NonEmptyStateDefinitionType StateDefinition
		{
			get
			{
				return this.stateDefinitionField;
			}
			set
			{
				this.stateDefinitionField = value;
			}
		}

		private string globalObjectIdField;

		private NonEmptyStateDefinitionType stateDefinitionField;
	}
}
