using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ProtectionRuleAndType
	{
		[XmlElement("AllInternal", typeof(string))]
		[XmlElement("And", typeof(ProtectionRuleAndType))]
		[XmlElement("SenderDepartments", typeof(ProtectionRuleSenderDepartmentsType))]
		[XmlElement("True", typeof(string))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("RecipientIs", typeof(ProtectionRuleRecipientIsType))]
		public object[] Items;

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType3[] ItemsElementName;
	}
}
