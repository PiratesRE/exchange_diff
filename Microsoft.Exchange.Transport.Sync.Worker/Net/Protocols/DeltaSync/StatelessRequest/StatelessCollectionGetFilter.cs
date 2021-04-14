using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[Serializable]
	public class StatelessCollectionGetFilter
	{
		[XmlElement("And", typeof(StatelessCollectionGetFilterAnd))]
		[XmlChoiceIdentifier("ItemElementName")]
		[XmlElement("Clause", typeof(Clause))]
		[XmlElement("Not", typeof(StatelessCollectionGetFilterNot))]
		[XmlElement("Or", typeof(StatelessCollectionGetFilterOR))]
		public object Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		[XmlIgnore]
		public ItemChoiceType ItemElementName
		{
			get
			{
				return this.itemElementNameField;
			}
			set
			{
				this.itemElementNameField = value;
			}
		}

		private object itemField;

		private ItemChoiceType itemElementNameField;
	}
}
