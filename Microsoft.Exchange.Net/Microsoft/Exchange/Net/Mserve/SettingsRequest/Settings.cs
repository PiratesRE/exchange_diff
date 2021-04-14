using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[XmlRoot(Namespace = "HMSETTINGS:", IsNullable = false)]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Settings
	{
		public ServiceSettingsType ServiceSettings
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

		public AccountSettingsType AccountSettings
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

		private ServiceSettingsType serviceSettingsField;

		private AccountSettingsType accountSettingsField;
	}
}
