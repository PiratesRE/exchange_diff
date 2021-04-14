using System;
using Microsoft.Exchange.VariantConfiguration.Reflection;

namespace Microsoft.Exchange.VariantConfiguration
{
	public sealed class VariantConfigurationSettingOverride : VariantConfigurationOverride
	{
		public VariantConfigurationSettingOverride(string componentName, string sectionName, params string[] parameters) : base(componentName, sectionName, parameters)
		{
		}

		public VariantConfigurationSettingOverride(VariantConfigurationSection section, params string[] parameters) : this(section.Name, section.SectionName, parameters)
		{
		}

		public override string FileName
		{
			get
			{
				return base.ComponentName + ".settings.ini";
			}
		}
	}
}
