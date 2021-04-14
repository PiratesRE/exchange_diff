using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "AddressesAndDomainsType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class AddressesAndDomainsType
	{
		[XmlIgnore]
		public AddressesType Addresses
		{
			get
			{
				if (this.internalAddresses == null)
				{
					this.internalAddresses = new AddressesType();
				}
				return this.internalAddresses;
			}
			set
			{
				this.internalAddresses = value;
			}
		}

		[XmlIgnore]
		public DomainsType Domains
		{
			get
			{
				if (this.internalDomains == null)
				{
					this.internalDomains = new DomainsType();
				}
				return this.internalDomains;
			}
			set
			{
				this.internalDomains = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AddressesType), ElementName = "Addresses", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AddressesType internalAddresses;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(DomainsType), ElementName = "Domains", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public DomainsType internalDomains;
	}
}
