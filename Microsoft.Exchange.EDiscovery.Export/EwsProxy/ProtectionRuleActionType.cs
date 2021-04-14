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
	public class ProtectionRuleActionType
	{
		[XmlElement("Argument")]
		public ProtectionRuleArgumentType[] Argument
		{
			get
			{
				return this.argumentField;
			}
			set
			{
				this.argumentField = value;
			}
		}

		[XmlAttribute]
		public ProtectionRuleActionKindType Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		private ProtectionRuleArgumentType[] argumentField;

		private ProtectionRuleActionKindType nameField;
	}
}
