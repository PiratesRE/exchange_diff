using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[XmlType(AnonymousType = true, Namespace = "HMMAIL:")]
	[XmlRoot(Namespace = "HMMAIL:", IsNullable = false)]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class Flag
	{
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("State", typeof(byte))]
		[XmlElement("Completed", typeof(byte))]
		[XmlElement("ReminderDate", typeof(string))]
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
		public ItemsChoiceType2[] ItemsElementName
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

		private ItemsChoiceType2[] itemsElementNameField;
	}
}
