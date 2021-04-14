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
	public class ProtectionRuleAndType
	{
		[XmlElement("AllInternal", typeof(string))]
		[XmlElement("True", typeof(string))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("And", typeof(ProtectionRuleAndType))]
		[XmlElement("RecipientIs", typeof(ProtectionRuleRecipientIsType))]
		[XmlElement("SenderDepartments", typeof(ProtectionRuleSenderDepartmentsType))]
		public object[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType3[] ItemsElementName
		{
			get
			{
				return this.itemsElementNameField;
			}
			set
			{
				this.itemsElementNameField = value;
			}
		}

		private object[] itemsField;

		private ItemsChoiceType3[] itemsElementNameField;
	}
}
