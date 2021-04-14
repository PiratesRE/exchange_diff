using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlRoot(ElementName = "Settings", Namespace = "HMSETTINGS:", IsNullable = false)]
	[Serializable]
	public class Settings
	{
		[XmlIgnore]
		public int Status
		{
			get
			{
				return this.internalStatus;
			}
			set
			{
				this.internalStatus = value;
				this.internalStatusSpecified = true;
			}
		}

		[XmlIgnore]
		public SettingsFault Fault
		{
			get
			{
				if (this.internalFault == null)
				{
					this.internalFault = new SettingsFault();
				}
				return this.internalFault;
			}
			set
			{
				this.internalFault = value;
			}
		}

		[XmlIgnore]
		public SettingsAuthPolicy AuthPolicy
		{
			get
			{
				if (this.internalAuthPolicy == null)
				{
					this.internalAuthPolicy = new SettingsAuthPolicy();
				}
				return this.internalAuthPolicy;
			}
			set
			{
				this.internalAuthPolicy = value;
			}
		}

		[XmlIgnore]
		public ServiceSettings ServiceSettings
		{
			get
			{
				if (this.internalServiceSettings == null)
				{
					this.internalServiceSettings = new ServiceSettings();
				}
				return this.internalServiceSettings;
			}
			set
			{
				this.internalServiceSettings = value;
			}
		}

		[XmlIgnore]
		public AccountSettings AccountSettings
		{
			get
			{
				if (this.internalAccountSettings == null)
				{
					this.internalAccountSettings = new AccountSettings();
				}
				return this.internalAccountSettings;
			}
			set
			{
				this.internalAccountSettings = value;
			}
		}

		[XmlElement(ElementName = "Status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalStatus;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalStatusSpecified;

		[XmlElement(Type = typeof(SettingsFault), ElementName = "Fault", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SettingsFault internalFault;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(SettingsAuthPolicy), ElementName = "AuthPolicy", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public SettingsAuthPolicy internalAuthPolicy;

		[XmlElement(Type = typeof(ServiceSettings), ElementName = "ServiceSettings", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ServiceSettings internalServiceSettings;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AccountSettings), ElementName = "AccountSettings", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AccountSettings internalAccountSettings;
	}
}
