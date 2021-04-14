using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class DeleteUMPromptsType : BaseRequestType
	{
		public string ConfigurationObject
		{
			get
			{
				return this.configurationObjectField;
			}
			set
			{
				this.configurationObjectField = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] PromptNames
		{
			get
			{
				return this.promptNamesField;
			}
			set
			{
				this.promptNamesField = value;
			}
		}

		private string configurationObjectField;

		private string[] promptNamesField;
	}
}
