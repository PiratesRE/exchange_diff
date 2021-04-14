using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "Properties", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Properties
	{
		[XmlIgnore]
		public PropertiesGet Get
		{
			get
			{
				if (this.internalGet == null)
				{
					this.internalGet = new PropertiesGet();
				}
				return this.internalGet;
			}
			set
			{
				this.internalGet = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(PropertiesGet), ElementName = "Get", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public PropertiesGet internalGet;
	}
}
