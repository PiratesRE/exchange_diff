using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "MailForwarding", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class MailForwarding
	{
		[XmlIgnore]
		public ForwardingMode Mode
		{
			get
			{
				return this.internalMode;
			}
			set
			{
				this.internalMode = value;
				this.internalModeSpecified = true;
			}
		}

		[XmlIgnore]
		public Addresses Addresses
		{
			get
			{
				if (this.internalAddresses == null)
				{
					this.internalAddresses = new Addresses();
				}
				return this.internalAddresses;
			}
			set
			{
				this.internalAddresses = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Mode", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public ForwardingMode internalMode;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalModeSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Addresses), ElementName = "Addresses", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public Addresses internalAddresses;
	}
}
