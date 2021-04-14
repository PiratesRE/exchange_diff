using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "ListsRequestTypeList", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class ListsRequestTypeList
	{
		[XmlIgnore]
		public string name
		{
			get
			{
				return this.internalname;
			}
			set
			{
				this.internalname = value;
			}
		}

		[XmlIgnore]
		public AddressesAndDomainsType Set
		{
			get
			{
				if (this.internalSet == null)
				{
					this.internalSet = new AddressesAndDomainsType();
				}
				return this.internalSet;
			}
			set
			{
				this.internalSet = value;
			}
		}

		[XmlIgnore]
		public AddressesAndDomainsType Add
		{
			get
			{
				if (this.internalAdd == null)
				{
					this.internalAdd = new AddressesAndDomainsType();
				}
				return this.internalAdd;
			}
			set
			{
				this.internalAdd = value;
			}
		}

		[XmlIgnore]
		public AddressesAndDomainsType Delete
		{
			get
			{
				if (this.internalDelete == null)
				{
					this.internalDelete = new AddressesAndDomainsType();
				}
				return this.internalDelete;
			}
			set
			{
				this.internalDelete = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.Unqualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalname;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AddressesAndDomainsType), ElementName = "Set", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AddressesAndDomainsType internalSet;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AddressesAndDomainsType), ElementName = "Add", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AddressesAndDomainsType internalAdd;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AddressesAndDomainsType), ElementName = "Delete", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AddressesAndDomainsType internalDelete;
	}
}
