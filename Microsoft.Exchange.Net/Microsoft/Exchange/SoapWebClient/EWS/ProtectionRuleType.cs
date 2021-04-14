using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ProtectionRuleType
	{
		public ProtectionRuleConditionType Condition;

		public ProtectionRuleActionType Action;

		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public bool UserOverridable;

		[XmlAttribute]
		public int Priority;
	}
}
