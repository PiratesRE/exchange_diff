using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[Serializable]
	public class ApplicationDataTypeRequestFlag
	{
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("ReminderDate", typeof(string))]
		[XmlElement("Completed", typeof(byte))]
		[XmlElement("State", typeof(byte))]
		[XmlElement("Title", typeof(stringWithCharSetType))]
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

		[XmlIgnore]
		[XmlElement("ItemsElementName")]
		public ItemsChoiceType[] ItemsElementName
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

		private ItemsChoiceType[] itemsElementNameField;
	}
}
