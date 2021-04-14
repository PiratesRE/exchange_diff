using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("New", "SettingOverride", SupportsShouldProcess = true)]
	public sealed class NewSettingOverride : NewOverrideBase
	{
		protected override bool IsFlight
		{
			get
			{
				return false;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string Component
		{
			get
			{
				return base.Fields["Component"] as string;
			}
			set
			{
				base.Fields["Component"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string Section
		{
			get
			{
				return base.Fields["Section"] as string;
			}
			set
			{
				base.Fields["Section"] = value;
			}
		}

		protected override SettingOverrideXml GetXml()
		{
			SettingOverrideXml xml = base.GetXml();
			xml.ComponentName = this.Component;
			xml.SectionName = this.Section;
			return xml;
		}

		protected override VariantConfigurationOverride GetOverride()
		{
			return new VariantConfigurationSettingOverride(this.Component, this.Section, base.Parameters.ToArray());
		}
	}
}
