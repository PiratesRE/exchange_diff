using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[DesignerCategory("code")]
	[Serializable]
	public class ListsRequestTypeList
	{
		[XmlElement("Add", typeof(AddressesAndDomainsType))]
		[XmlElement("Delete", typeof(AddressesAndDomainsType))]
		[XmlElement("Set", typeof(AddressesAndDomainsType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		public AddressesAndDomainsType[] Items
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

		[XmlAttribute]
		public string name
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

		private AddressesAndDomainsType[] itemsField;

		private ItemsChoiceType[] itemsElementNameField;

		private string nameField;
	}
}
