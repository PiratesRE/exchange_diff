using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlRoot(ElementName = "Settings", Namespace = "HMSETTINGS:", IsNullable = false)]
	[Serializable]
	public class Settings
	{
		[XmlIgnore]
		public ServiceSettingsType ServiceSettings
		{
			get
			{
				if (this.internalServiceSettings == null)
				{
					this.internalServiceSettings = new ServiceSettingsType();
				}
				return this.internalServiceSettings;
			}
			set
			{
				this.internalServiceSettings = value;
			}
		}

		[XmlIgnore]
		public AccountSettingsType AccountSettings
		{
			get
			{
				if (this.internalAccountSettings == null)
				{
					this.internalAccountSettings = new AccountSettingsType();
				}
				return this.internalAccountSettings;
			}
			set
			{
				this.internalAccountSettings = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ServiceSettingsType), ElementName = "ServiceSettings", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public ServiceSettingsType internalServiceSettings;

		[XmlElement(Type = typeof(AccountSettingsType), ElementName = "AccountSettings", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public AccountSettingsType internalAccountSettings;
	}
}
