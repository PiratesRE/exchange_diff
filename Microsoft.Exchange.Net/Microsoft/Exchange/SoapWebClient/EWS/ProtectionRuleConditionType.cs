using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ProtectionRuleConditionType
	{
		[XmlElement("AllInternal", typeof(string))]
		[XmlElement("And", typeof(ProtectionRuleAndType))]
		[XmlElement("SenderDepartments", typeof(ProtectionRuleSenderDepartmentsType))]
		[XmlElement("RecipientIs", typeof(ProtectionRuleRecipientIsType))]
		[XmlElement("True", typeof(string))]
		[XmlChoiceIdentifier("ItemElementName")]
		public object Item;

		[XmlIgnore]
		public ItemChoiceType ItemElementName;
	}
}
