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
	public class VotingOptionDataType
	{
		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public SendPromptType SendPrompt
		{
			get
			{
				return this.sendPromptField;
			}
			set
			{
				this.sendPromptField = value;
			}
		}

		[XmlIgnore]
		public bool SendPromptSpecified
		{
			get
			{
				return this.sendPromptFieldSpecified;
			}
			set
			{
				this.sendPromptFieldSpecified = value;
			}
		}

		private string displayNameField;

		private SendPromptType sendPromptField;

		private bool sendPromptFieldSpecified;
	}
}
