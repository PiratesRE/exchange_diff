using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DesignerCategory("code")]
	[XmlRoot(Namespace = "HMSETTINGS:", IsNullable = false)]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Settings
	{
		public int Status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		public SettingsFault Fault
		{
			get
			{
				return this.faultField;
			}
			set
			{
				this.faultField = value;
			}
		}

		public SettingsAuthPolicy AuthPolicy
		{
			get
			{
				return this.authPolicyField;
			}
			set
			{
				this.authPolicyField = value;
			}
		}

		public SettingsServiceSettings ServiceSettings
		{
			get
			{
				return this.serviceSettingsField;
			}
			set
			{
				this.serviceSettingsField = value;
			}
		}

		public SettingsAccountSettings AccountSettings
		{
			get
			{
				return this.accountSettingsField;
			}
			set
			{
				this.accountSettingsField = value;
			}
		}

		private int statusField;

		private SettingsFault faultField;

		private SettingsAuthPolicy authPolicyField;

		private SettingsServiceSettings serviceSettingsField;

		private SettingsAccountSettings accountSettingsField;
	}
}
